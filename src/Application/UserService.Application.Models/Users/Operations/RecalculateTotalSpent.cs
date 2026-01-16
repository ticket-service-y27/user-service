using UserService.Application.Models.Users.Enums;

namespace UserService.Application.Models.Users.Operations;

public record RecalculateTotalSpent(
    long NewPeriodStartTotalSpent,
    long NewPeriodEndTotalSpent,
    long PeriodTotalSpent,
    UserLoyaltyLevel LoyaltyLevel);