namespace UserService.Application.Models.Users;

public record UserLoyaltyPeriodState(
    long UserId,
    DateTimeOffset PeriodStartAt,
    long PeriodStartTotalSpent,
    long PeriodEndTotalSpent,
    DateTimeOffset CalculatedAt);