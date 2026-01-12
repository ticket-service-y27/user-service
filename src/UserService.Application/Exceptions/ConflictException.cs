namespace UserService.Application.Exceptions;

public class ConflictException : Exception
{
    public string FieldName { get; }

    public string FieldValue { get; }

    public ConflictException(string fieldName, string fieldValue)
        : base($"Exception. User with field {fieldName}={fieldValue} already exists")
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }
}