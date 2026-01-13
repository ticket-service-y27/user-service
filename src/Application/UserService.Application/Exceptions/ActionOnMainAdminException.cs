namespace UserService.Application.Exceptions;

public class ActionOnMainAdminException : Exception
{
    public ActionOnMainAdminException()
        : base($"Exception. Forbidden block and assign role for MAIN ADMIN") { }
}