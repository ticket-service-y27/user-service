namespace UserService.Application.Exceptions;

public class UserBlockedException : Exception
{
    public string Nickname { get; }

    public long UserId { get; }

    public UserBlockedException(string nickname, long userId)
        : base($"Exception. User {nickname} with id={userId} is blocked")
    {
        Nickname = nickname;
        UserId = userId;
    }
}