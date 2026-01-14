using Grpc.Core;
using Users.UserService.Contracts;
using UserService.Application.Contracts;

namespace UserService.Presentation.Grpc.Services;

public class UserServiceGrpc : Users.UserService.Contracts.UserService.UserServiceBase
{
    private readonly IUserService _userService;

    public UserServiceGrpc(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<LogInByNicknameResponse> LogInByNickname(
        LogInByNicknameRequest request,
        ServerCallContext context)
    {
        return new LogInByNicknameResponse
        {
            JwtAccessToken = await _userService.LogInByNicknameAsync(
                request.Nickname,
                request.Password,
                context.CancellationToken),
        };
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        return new CreateUserResponse
        {
            UserId = await _userService.CreateUserAsync(
                request.Nickname,
                request.Email,
                request.Password,
                context.CancellationToken),
        };
    }

    public override async Task<AssignUserRoleResponse> AssignUserRole(
        AssignUserRoleRequest request,
        ServerCallContext context)
    {
        await _userService.AssignUserRoleAsync(request.UserId, request.Role.MapUserRole(), context.CancellationToken);
        return new AssignUserRoleResponse();
    }

    public override async Task<BlockUserByIdResponse> BlockUserById(
        BlockUserByIdRequest request,
        ServerCallContext context)
    {
        await _userService.BlockUserByIdAsync(request.UserId, context.CancellationToken);
        return new BlockUserByIdResponse();
    }
}