using UserService.Application.Models.Users;
using UserService.Application.Models.Users.Operations;

namespace UserService.Application.Abstractions.LoyaltySystem.Managers;

public interface IUserLoyaltyManager
{
    RecalculateTotalSpent RecalculateTotalSpentAsync(
        long userId,
        long totalSpent,
        DateTimeOffset calculatedAt,
        UserLoyaltyPeriodState periodState,
        CancellationToken ct);
}