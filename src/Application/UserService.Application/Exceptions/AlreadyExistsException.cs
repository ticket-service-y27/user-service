namespace UserService.Application.Exceptions;

public class AlreadyExistsException : Exception
{
    public string FieldName { get; }

    public string FieldValue { get; }

    public AlreadyExistsException(string fieldName, string fieldValue)
        : base($"Exception. User with {fieldName}={fieldValue} already exists")
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }
}