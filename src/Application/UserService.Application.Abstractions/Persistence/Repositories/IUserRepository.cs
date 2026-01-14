using UserService.Application.Models.Users;

namespace UserService.Application.Abstractions.Persistence.Repositories;

public interface IUserRepository
{
    Task<long> CreateAsync(
        string nickname,
        string email,
        string passwordHash,
        UserRole role,
        CancellationToken ct);

    Task<bool> AssignUserRoleAsync(long userId, UserRole role, CancellationToken ct);

    Task<bool> BlockUserByIdAsync(long userId, CancellationToken ct);

    Task<bool> UnblockUserByIdAsync(long userId, CancellationToken ct);

    Task<User?> GetUserByNicknameAsync(string nickname, CancellationToken ct);

    Task<User?> GetUserByIdAsync(long userId, CancellationToken ct);
}