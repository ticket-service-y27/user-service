using Itmo.Dev.Platform.Events;
using System.Net.Mail;
using System.Transactions;
using UserService.Application.Abstractions.LoyaltySystem;
using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Abstractions.Security;
using UserService.Application.Contracts;
using UserService.Application.Contracts.Users.Events;
using UserService.Application.Exceptions;
using UserService.Application.Models.Users;
using UserService.Application.Models.Users.Enums;

namespace UserService.Application.Services;

public class UserApplicationService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserLoyaltyAccountRepository _userLoyaltyAccountRepository;
    private readonly IUserLoyaltyPeriodRepository _userLoyaltyPeriodRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenCreator _jwtTokenCreator;
    private readonly IUserLoyaltyManager _userLoyaltyManager;
    private readonly IEventPublisher _eventPublisher;

    public UserApplicationService(
        IUserRepository userRepository,
        IUserLoyaltyAccountRepository userLoyaltyAccountRepository,
        IUserLoyaltyPeriodRepository userLoyaltyPeriodRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenCreator jwtTokenCreator,
        IUserLoyaltyManager userLoyaltyManager,
        IEventPublisher eventPublisher)
    {
        _userRepository = userRepository;
        _userLoyaltyAccountRepository = userLoyaltyAccountRepository;
        _userLoyaltyPeriodRepository = userLoyaltyPeriodRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenCreator = jwtTokenCreator;
        _userLoyaltyManager = userLoyaltyManager;
        _eventPublisher = eventPublisher;
    }

    public async Task<long> CreateUserAsync(string nickname, string email, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new FieldValidationException(nameof(nickname), nickname);
        if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out _))
            throw new FieldValidationException(nameof(email), email);
        if (string.IsNullOrWhiteSpace(password))
            throw new FieldValidationException(nameof(password), password);

        nickname = nickname.Trim();
        if (await _userRepository.GetByNicknameAsync(nickname, ct) is not null)
            throw new AlreadyExistsException(nameof(nickname), nickname);

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        long userId = await _userRepository.CreateAsync(
            nickname,
            email.Trim().ToLowerInvariant(),
            _passwordHasher.Hash(password),
            UserRole.User,
            ct);
        await _userLoyaltyAccountRepository.CreateAsync(userId, ct);
        await _userLoyaltyPeriodRepository.CreateAsync(userId, ct);
        await _eventPublisher.PublishAsync(
            new UserCreatedEvent(userId, DateTimeOffset.UtcNow),
            ct);

        transactionScope.Complete();

        return userId;
    }

    public async Task AssignUserRoleAsync(long userId, UserRole role, CancellationToken ct)
    {
        if (userId == 1)
            throw new ActionOnMainAdminException();

        User? user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            throw new NotFoundException(nameof(User), nameof(userId), userId.ToString());
        if (user.IsBlocked)
            throw new UserBlockedException(user.Nickname, user.Id);

        await _userRepository.AssignUserRoleAsync(userId, role, ct);
    }

    public async Task BlockUserByIdAsync(long userId, CancellationToken ct)
    {
        if (userId == 1)
            throw new ActionOnMainAdminException();

        User? user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            throw new NotFoundException(nameof(User), nameof(userId), userId.ToString());
        if (user.IsBlocked)
            throw new UserAlreadyBlockedException(user.Nickname, user.Id);

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _userRepository.BlockByIdAsync(userId, ct);
        if (!await _userLoyaltyAccountRepository.SetBlockedAsync(userId, true, ct))
            throw new NotFoundException(nameof(UserLoyaltyAccount), nameof(userId), userId.ToString());
        await _eventPublisher.PublishAsync(
            new UserBlockedEvent(userId, DateTimeOffset.UtcNow),
            ct);

        transactionScope.Complete();
    }

    public async Task UnblockUserByIdAsync(long userId, CancellationToken ct)
    {
        if (userId == 1)
            throw new ActionOnMainAdminException();

        User? user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            throw new NotFoundException(nameof(User), nameof(userId), userId.ToString());
        if (!user.IsBlocked)
            throw new UserAlreadyUnblockedException(user.Nickname, user.Id);

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _userRepository.UnblockByIdAsync(userId, ct);
        if (!await _userLoyaltyAccountRepository.SetBlockedAsync(userId, false, ct))
            throw new NotFoundException(nameof(UserLoyaltyAccount), nameof(userId), userId.ToString());
        await _eventPublisher.PublishAsync(
            new UserUnblockedEvent(userId, DateTimeOffset.UtcNow),
            ct);

        transactionScope.Complete();
    }

    public async Task<string> LogInByNicknameAsync(string nickname, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new FieldValidationException(nameof(nickname), nickname);
        if (string.IsNullOrWhiteSpace(password))
            throw new FieldValidationException(nameof(password), password);

        User? user = await _userRepository.GetByNicknameAsync(nickname.Trim(), ct);
        if (user is null)
            throw new InvalidAuthorizeException();
        if (user.IsBlocked)
            throw new UserBlockedException(nickname, user.Id);

        bool isAuthorize = _passwordHasher.Verify(password, user.PasswordHash);
        if (!isAuthorize)
            throw new InvalidAuthorizeException();

        return _jwtTokenCreator.CreateAccessToken(user.Id, user.Role);
    }

    public async Task RecalculateUserLoyaltyAsync(
        long userId,
        long totalSpent,
        DateTimeOffset calculatedAt,
        CancellationToken ct)
    {
        UserLoyaltyPeriodState? periodState = await _userLoyaltyPeriodRepository.GetAsync(userId, ct);
        if (periodState is null)
            throw new NotFoundException(nameof(UserLoyaltyPeriodState), nameof(userId), userId.ToString());
        if (calculatedAt <= periodState.CalculatedAt)
            return;

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        bool isUpdated = await _userLoyaltyManager.RecalculateAsync(userId, totalSpent, calculatedAt, periodState, ct);
        if (isUpdated)
            transactionScope.Complete();
    }
}