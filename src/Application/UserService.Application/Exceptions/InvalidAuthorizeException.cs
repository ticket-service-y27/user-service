namespace UserService.Application.Exceptions;

public class InvalidAuthorizeException : Exception
{
    public InvalidAuthorizeException()
        : base("Exception. Invalid nickname or password") { }
}