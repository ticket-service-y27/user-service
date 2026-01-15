using UserService.Application.Models.Users;

namespace UserService.Application.Abstractions.Persistence.Repositories;

public interface IUserLoyaltyPeriodRepository
{
    Task CreateAsync(long userId, CancellationToken ct);

    Task<bool> UpdateAsync(
        long userId,
        DateTimeOffset periodStartAt,
        long periodStartTotalSpent,
        long periodEndTotalSpent,
        DateTimeOffset calculatedAt,
        CancellationToken ct);

    Task<UserLoyaltyPeriodState?> GetAsync(long userId, CancellationToken ct);
}