using Itmo.Dev.Platform.Events;
using System.Net.Mail;
using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Abstractions.Security;
using UserService.Application.Contracts;
using UserService.Application.Contracts.Users.Events;
using UserService.Application.Exceptions;
using UserService.Application.Models.Users;

namespace UserService.Application.Services;

public class UserApplicationService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenCreator _jwtTokenCreator;
    private readonly IEventPublisher _eventPublisher;

    public UserApplicationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenCreator jwtTokenCreator,
        IEventPublisher eventPublisher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenCreator = jwtTokenCreator;
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
        if (await _userRepository.GetUserByNicknameAsync(nickname, ct) is not null)
            throw new AlreadyExistsException(nameof(nickname), nickname);

        long userId = await _userRepository.CreateAsync(
            nickname,
            email.Trim().ToLowerInvariant(),
            _passwordHasher.Hash(password),
            UserRole.User,
            ct);

        await _eventPublisher.PublishAsync(
            new UserCreatedEvent(userId, DateTimeOffset.UtcNow),
            ct);

        return userId;
    }

    public async Task AssignUserRoleAsync(long userId, UserRole role, CancellationToken ct)
    {
        if (userId == 1)
            throw new ActionOnMainAdminException();

        User? user = await _userRepository.GetUserByIdAsync(userId, ct);
        if (user is null)
            throw new NotFoundException(nameof(userId), userId.ToString());
        if (user.IsBlocked)
            throw new UserBlockedException(user.Nickname, user.Id);

        await _userRepository.AssignUserRoleAsync(userId, role, ct);
    }

    public async Task BlockUserByIdAsync(long userId, CancellationToken ct)
    {
        if (userId == 1)
            throw new ActionOnMainAdminException();

        User? user = await _userRepository.GetUserByIdAsync(userId, ct);
        if (user is null)
            throw new NotFoundException(nameof(userId), userId.ToString());
        if (user.IsBlocked)
            throw new UserBlockedException(user.Nickname, user.Id);

        await _userRepository.BlockUserByIdAsync(userId, ct);
    }

    public async Task<string> LogInByNicknameAsync(string nickname, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new FieldValidationException(nameof(nickname), nickname);
        if (string.IsNullOrWhiteSpace(password))
            throw new FieldValidationException(nameof(password), password);

        User? user = await _userRepository.GetUserByNicknameAsync(nickname.Trim(), ct);
        if (user is null)
            throw new InvalidAuthorizeException();
        if (user.IsBlocked)
            throw new UserBlockedException(nickname, user.Id);

        bool isAuthorize = _passwordHasher.Verify(password, user.PasswordHash);
        if (!isAuthorize)
            throw new InvalidAuthorizeException();

        return _jwtTokenCreator.CreateAccessToken(user.Id, user.Role);
    }
}