namespace UserService.Application.Exceptions;

public class UserAlreadyUnblockedException : Exception
{
    public string Nickname { get; }

    public long UserId { get; }

    public UserAlreadyUnblockedException(string nickname, long userId)
        : base($"Exception. User {nickname} with id={userId} is already unblocked")
    {
        Nickname = nickname;
        UserId = userId;
    }
}