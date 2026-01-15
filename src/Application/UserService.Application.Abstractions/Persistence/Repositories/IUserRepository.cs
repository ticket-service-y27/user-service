using UserService.Application.Models.Users;
using UserService.Application.Models.Users.Enums;

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

    Task<bool> BlockByIdAsync(long userId, CancellationToken ct);

    Task<bool> UnblockByIdAsync(long userId, CancellationToken ct);

    Task<User?> GetByNicknameAsync(string nickname, CancellationToken ct);

    Task<User?> GetByIdAsync(long userId, CancellationToken ct);
}