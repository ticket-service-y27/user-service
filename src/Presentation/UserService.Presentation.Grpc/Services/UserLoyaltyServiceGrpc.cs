using Grpc.Core;
using Users.UserService.Contracts;
using UserService.Application.Contracts;
using UserService.Application.Models.Users.Dtos;

namespace UserService.Presentation.Grpc.Services;

public class UserLoyaltyServiceGrpc : Users.UserService.Contracts.UserLoyaltyService.UserLoyaltyServiceBase
{
    private readonly IUserService _userService;

    public UserLoyaltyServiceGrpc(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<GetUserDiscountResponse> GetUserDiscount(GetUserDiscountRequest request, ServerCallContext context)
    {
        UserDiscountInfoDto state = await _userService.GetUserDiscountInfoAsync(
            request.UserId,
            context.CancellationToken);

        return new GetUserDiscountResponse
        {
            DiscountPercent = state.DiscountPercent,
            IsBlocked = state.IsBlocked,
        };
    }
}