namespace UserService.Application.Exceptions;

public class NotFoundException : Exception
{
    public string Entity { get; }

    public string FieldName { get; }

    public string FieldValue { get; }

    public NotFoundException(string entity, string fieldName, string fieldValue)
        : base($"Exception. {entity} with {fieldName}={fieldValue} not found")
    {
        Entity = entity;
        FieldName = fieldName;
        FieldValue = fieldValue;
    }
}