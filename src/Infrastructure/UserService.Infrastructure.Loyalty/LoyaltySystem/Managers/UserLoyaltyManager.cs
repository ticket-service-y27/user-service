using UserService.Application.Abstractions.LoyaltySystem;
using UserService.Application.Abstractions.LoyaltySystem.Managers;
using UserService.Application.Models.Users;
using UserService.Application.Models.Users.Enums;
using UserService.Application.Models.Users.Operations;

namespace UserService.Infrastructure.Loyalty.LoyaltySystem.Managers;

public class UserLoyaltyManager : IUserLoyaltyManager
{
    private readonly ILoyaltyLevelCalculator _loyaltyLevelCalculator;

    public UserLoyaltyManager(
        ILoyaltyLevelCalculator loyaltyLevelCalculator)
    {
        _loyaltyLevelCalculator = loyaltyLevelCalculator;
    }

    public RecalculateTotalSpent RecalculateTotalSpent(
        long userId,
        long totalSpent,
        DateTimeOffset calculatedAt,
        UserLoyaltyPeriodState periodState)
    {
        long newPeriodStartTotalSpent = periodState.PeriodStartTotalSpent;
        long newPeriodEndTotalSpent = totalSpent;

        long periodTotalSpent = totalSpent - periodState.PeriodStartTotalSpent;
        UserLoyaltyLevel loyaltyLevel = _loyaltyLevelCalculator.CalculateLoyaltyLevel(periodTotalSpent);

        if (totalSpent < periodState.PeriodEndTotalSpent)
        {
            newPeriodStartTotalSpent = totalSpent;
            newPeriodEndTotalSpent = totalSpent;
            periodTotalSpent = 0;
            loyaltyLevel = UserLoyaltyLevel.Bronze;
        }

        return new RecalculateTotalSpent(
            newPeriodStartTotalSpent,
            newPeriodEndTotalSpent,
            periodTotalSpent,
            loyaltyLevel);
    }
}