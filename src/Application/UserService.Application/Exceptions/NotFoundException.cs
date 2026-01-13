namespace UserService.Application.Exceptions;

public class NotFoundException : Exception
{
    public string FieldName { get; }

    public string FieldValue { get; }

    public NotFoundException(string fieldName, string fieldValue)
        : base($"Exception. User with {fieldName}={fieldValue} not found")
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }
}