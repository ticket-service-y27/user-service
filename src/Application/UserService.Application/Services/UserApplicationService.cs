using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Abstractions.Security;
using UserService.Application.Contracts;
using UserService.Application.Exceptions;
using UserService.Application.Models.Users;

namespace UserService.Application.Services;

public class UserApplicationService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenCreator _jwtTokenCreator;

    public UserApplicationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenCreator jwtTokenCreator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenCreator = jwtTokenCreator;
    }

    public async Task<long> CreateUserAsync(string nickname, string email, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new FieldValidationException(nameof(nickname), nickname);
        if (string.IsNullOrWhiteSpace(email))
            throw new FieldValidationException(nameof(email), email);
        if (string.IsNullOrWhiteSpace(password))
            throw new FieldValidationException(nameof(password), password);

        nickname = nickname.Trim();
        if (await _userRepository.GetUserByNicknameAsync(nickname, ct) is not null)
            throw new AlreadyExistsException(nameof(nickname), nickname);

        return await _userRepository.CreateAsync(
            nickname,
            email.Trim().ToLowerInvariant(),
            _passwordHasher.Hash(password),
            UserRole.User,
            ct);
    }

    public async Task AssignUserRoleAsync(long userId, UserRole role, CancellationToken ct)
    {
        await _userRepository.AssignUserRoleAsync(userId, role, ct);
    }

    public async Task BlockUserByIdAsync(long userId, CancellationToken ct)
    {
        if (userId == 1)
            return;

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