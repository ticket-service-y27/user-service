using Grpc.Core;
using Users.UserService.Contracts;
using UserService.Application.Models.Users.Enums;
using UserLoyaltyLevel = UserService.Application.Models.Users.Enums.UserLoyaltyLevel;

namespace UserService.Presentation.Grpc;

public static class Mappings
{
    public static UserRole MapUserRole(this UserRoleGrpc role)
    {
        return role switch
        {
            UserRoleGrpc.User => UserRole.User,
            UserRoleGrpc.Admin => UserRole.Admin,
            UserRoleGrpc.Organizer => UserRole.Organizer,
            UserRoleGrpc.Unspecified =>
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Exception. Role is required")),
            _ => throw new RpcException(new Status(StatusCode.InvalidArgument, "Exception. Role is required")),
        };
    }

    public static UserLoyaltyLevelGrpc MapUserLoyaltyLevel(this UserLoyaltyLevel level)
    {
        return level switch
        {
            UserLoyaltyLevel.Bronze => UserLoyaltyLevelGrpc.Bronze,
            UserLoyaltyLevel.Silver => UserLoyaltyLevelGrpc.Silver,
            UserLoyaltyLevel.Gold => UserLoyaltyLevelGrpc.Gold,
            UserLoyaltyLevel.Platinum => UserLoyaltyLevelGrpc.Platinum,
            _ => UserLoyaltyLevelGrpc.Unspecified,
        };
    }
}