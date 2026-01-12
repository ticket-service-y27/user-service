using UserService.Application.Models.Users;

namespace UserService.Application.Contracts;

public interface IUserService
{
    Task<long> CreateUserAsync(string nickname, string email, string password, CancellationToken ct);

    Task AssignUserRoleAsync(long userId, UserRole role, CancellationToken ct);

    Task BlockUserByIdAsync(long userId, CancellationToken ct);

    Task<long> LogInByNicknameAsync(string nickname, string password, CancellationToken ct);
}