using UserService.Application.Abstractions.LoyaltySystem.Managers;
using UserService.Application.Abstractions.Persistence.Repositories;
using UserService.Application.Models.Users;
using UserService.Application.Models.Users.Enums;

namespace UserService.Infrastructure.Loyalty.LoyaltySystem.Managers;

public class LoyaltyPeriodManager : ILoyaltyPeriodManager
{
    private readonly ILoyaltyPeriodRepository _loyaltyPeriodRepository;
    private readonly IUserLoyaltyAccountRepository _userLoyaltyAccountRepository;

    public LoyaltyPeriodManager(
        ILoyaltyPeriodRepository loyaltyPeriodRepository,
        IUserLoyaltyAccountRepository userLoyaltyAccountRepository)
    {
        _loyaltyPeriodRepository = loyaltyPeriodRepository;
        _userLoyaltyAccountRepository = userLoyaltyAccountRepository;
    }

    public async Task MoveExpiredPeriodsAsync(DateTimeOffset timeNow, TimeSpan periodLength, CancellationToken ct)
    {
        IReadOnlyList<long> userIds =
            await _loyaltyPeriodRepository.FindUserIdsWithExpiredPeriodAsync(timeNow, periodLength, ct);

        foreach (long userId in userIds)
        {
            UserLoyaltyPeriodState? state = await _loyaltyPeriodRepository.GetAsync(userId, ct);
            if (state is null)
                continue;

            long newStartTotalSpent = state.PeriodEndTotalSpent;

            await _loyaltyPeriodRepository.UpdateAsync(
                userId,
                periodStartAt: timeNow,
                periodStartTotalSpent: newStartTotalSpent,
                periodEndTotalSpent: newStartTotalSpent,
                calculatedAt: timeNow,
                ct);
            await _userLoyaltyAccountRepository.UpdateLoyaltyLevelAsync(
                userId,
                periodTotalSpent: 0,
                loyaltyLevel: UserLoyaltyLevel.Bronze,
                calculatedAt: timeNow,
                ct);
        }
    }
}