namespace UserService.Application.Exceptions;

public class UserAlreadyBlockedException : Exception
{
    public string Nickname { get; }

    public long UserId { get; }

    public UserAlreadyBlockedException(string nickname, long userId)
        : base($"Exception. User {nickname} with id={userId} is already blocked")
    {
        Nickname = nickname;
        UserId = userId;
    }
}