using UserService.Application.Models.Users.Enums;

namespace UserService.Application.Contracts;

public interface IUserService
{
    Task<long> CreateUserAsync(string nickname, string email, string password, CancellationToken ct);

    Task AssignUserRoleAsync(long userId, UserRole role, CancellationToken ct);

    Task BlockUserByIdAsync(long userId, CancellationToken ct);

    Task UnblockUserByIdAsync(long userId, CancellationToken ct);

    Task<string> LogInByNicknameAsync(string nickname, string password, CancellationToken ct);

    Task RecalculateUserLoyaltyAsync(
        long userId,
        long totalSpent,
        DateTimeOffset calculatedAt,
        CancellationToken ct);
}