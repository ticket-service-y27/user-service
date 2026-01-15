using UserService.Application.Models.Users.Enums;

namespace UserService.Application.Models.Users;

public record UserLoyaltyState(UserLoyaltyLevel Level, bool IsBlocked);