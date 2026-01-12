using Grpc.Core;
using UserService.Application.Models.Users;
using UserService.Users.Contracts;

namespace UserService.Presentation.Grpc;

public static class Map
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
}