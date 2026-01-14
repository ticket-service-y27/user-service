namespace UserService.Application.Exceptions;

public class ActionOnMainAdminException : Exception
{
    public ActionOnMainAdminException()
        : base($"Exception. Forbidden any actions for MAIN ADMIN") { }
}