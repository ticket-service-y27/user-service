using UserService.Application.Models.Users.Enums;

namespace UserService.Application.Models.Users;

public record UserLoyaltyAccount(
    long UserId,
    long TotalSpent,
    UserLoyaltyLevel LoyaltyLevel,
    DateTimeOffset CalculatedAt,
    bool IsBlocked,
    DateTimeOffset? BlockedAt);