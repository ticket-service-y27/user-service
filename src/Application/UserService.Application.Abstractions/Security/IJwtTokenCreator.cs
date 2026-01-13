using UserService.Application.Models.Users;

namespace UserService.Application.Abstractions.Security;

public interface IJwtTokenCreator
{
    string CreateAccessToken(long userId, UserRole role);
}