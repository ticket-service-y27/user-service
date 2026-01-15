using UserService.Application.Models.Users.Enums;

namespace UserService.Infrastructure.Loyalty.LoyaltySystem.Options;

public class LoyaltyLevelOptions
{
    public UserLoyaltyLevel LoyaltyLevel { get; set; }

    public long MinPeriodTotalSpent { get; set; }

    public int DiscountPercent { get; set; }
}