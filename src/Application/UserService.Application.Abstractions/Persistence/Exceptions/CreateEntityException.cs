namespace UserService.Application.Abstractions.Persistence.Exceptions;

public class CreateEntityException : Exception
{
    public string EntityName { get; }

    public CreateEntityException(string entityName)
        : base($"Exception. Failed to create {entityName}")
    {
        EntityName = entityName;
    }
}