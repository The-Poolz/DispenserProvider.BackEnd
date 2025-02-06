namespace DispenserProvider.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ErrorAttribute(string errorMessage) : Attribute
{
    public string Message { get; } = errorMessage;
}