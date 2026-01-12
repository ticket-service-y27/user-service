namespace UserService.Application.Exceptions;

public sealed class FieldValidationException : Exception
{
    public string FieldName { get; }

    public string FieldValue { get; }

    public FieldValidationException(string fieldName, string fieldValue)
        : base($"Exception. Field {fieldName}={fieldValue} is required")
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }
}