using UserService.Application.Models.Users.Enums;

namespace UserService.Application.Models.Users.Operations;

public record UserLoyaltyState(UserLoyaltyLevel Level, bool IsBlocked);