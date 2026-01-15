using UserService.Application.Models.Users;

namespace UserService.Application.Abstractions.LoyaltySystem;

public interface IUserLoyaltyManager
{
    Task<bool> RecalculateAsync(
        long userId,
        long totalSpent,
        DateTimeOffset calculatedAt,
        UserLoyaltyPeriodState periodState,
        CancellationToken ct);
}