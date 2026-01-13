namespace UserService.Application.Models.Users;

public sealed record User(
    long Id,
    string Nickname,
    string Email,
    string PasswordHash,
    UserRole Role,
    DateTimeOffset CreatedAt,
    bool IsBlocked,
    DateTimeOffset? BlockedAt);