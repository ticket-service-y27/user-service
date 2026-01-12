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

    public UserApplicationService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
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
            throw new ConflictException(nameof(nickname), nickname);

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
        await _userRepository.BlockUserByIdAsync(userId, ct);
    }

    public async Task<long> LogInByNicknameAsync(string nickname, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new FieldValidationException(nameof(nickname), nickname);
        if (string.IsNullOrWhiteSpace(password))
            throw new FieldValidationException(nameof(password), password);

        User? user = await _userRepository.GetUserByNicknameAsync(nickname.Trim(), ct);
        if (user is null)
            throw new InvalidAuthorizeException();

        bool isAuthorize = _passwordHasher.Verify(password, user.PasswordHash);
        if (!isAuthorize)
            throw new InvalidAuthorizeException();

        return user.Id;
    }
}