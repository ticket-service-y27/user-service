using UserService.Application.Models.Users.Enums;
using UserService.Application.Models.Users.Operations;

namespace UserService.Application.Abstractions.Persistence.Repositories;

public interface IUserLoyaltyAccountRepository
{
    Task CreateAsync(long userId, CancellationToken ct);

    Task<bool> SetBlockedAsync(long userId, bool isBlocked, CancellationToken ct);

    Task<bool> UpdateLoyaltyLevelAsync(
        long userId,
        long periodTotalSpent,
        UserLoyaltyLevel loyaltyLevel,
        DateTimeOffset calculatedAt,
        CancellationToken ct);

    Task<UserLoyaltyState?> GetUserLoyaltyStateAsync(long userId, CancellationToken ct);
}