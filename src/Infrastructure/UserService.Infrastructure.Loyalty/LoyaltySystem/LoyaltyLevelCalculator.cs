#pragma warning disable IDE0072
using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.LoyaltySystem;
using UserService.Application.Models.Users.Enums;
using UserService.Infrastructure.Loyalty.LoyaltySystem.Options;

namespace UserService.Infrastructure.Loyalty.LoyaltySystem;

public class LoyaltyLevelCalculator : ILoyaltyLevelCalculator
{
    private readonly LoyaltyLevelCalculatorOptions _options;

    public LoyaltyLevelCalculator(IOptions<LoyaltyLevelCalculatorOptions> options)
    {
        _options = options.Value;
    }

    public UserLoyaltyLevel CalculateLoyaltyLevel(long periodTotalSpent)
    {
        IOrderedEnumerable<LoyaltyLevelOptions> levels = _options.Levels.OrderBy(
            t => t.MinPeriodTotalSpent);

        UserLoyaltyLevel levelFinal = UserLoyaltyLevel.Bronze;

        foreach (LoyaltyLevelOptions? l in levels)
        {
            if (periodTotalSpent >= l.MinPeriodTotalSpent)
                levelFinal = l.LoyaltyLevel;
        }

        return levelFinal;
    }

    public int GetDiscountPercent(UserLoyaltyLevel level)
    {
        foreach (LoyaltyLevelOptions l in _options.Levels)
        {
            if (level == l.LoyaltyLevel)
                return l.DiscountPercent;
        }

        return 0;
    }
}