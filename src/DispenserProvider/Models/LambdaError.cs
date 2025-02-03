using FluentValidation;
using Newtonsoft.Json.Linq;

namespace DispenserProvider.Models;

public class LambdaError
{
    public LambdaError(Exception exception)
    {
        ErrorType = exception.GetType().Name;
        ErrorMessage = exception.Message;
    }

    public LambdaError(ValidationException exception)
    {
        var error = exception.Errors.First();
        ErrorType = exception.GetType().Name;
        ErrorMessage = error.ErrorMessage;
        Data = error.CustomState == null ? null : JObject.FromObject(error.CustomState);
    }

    public string ErrorMessage { get; }
    public string ErrorType { get; }
    public JObject? Data { get; }
}