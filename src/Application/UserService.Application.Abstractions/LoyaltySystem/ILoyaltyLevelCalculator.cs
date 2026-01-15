using UserService.Application.Models.Users.Enums;

namespace UserService.Application.Abstractions.LoyaltySystem;

public interface ILoyaltyLevelCalculator
{
    UserLoyaltyLevel CalculateLoyaltyLevel(long periodTotalSpent);

    int GetDiscountPercent(UserLoyaltyLevel level);
}