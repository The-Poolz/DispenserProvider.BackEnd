using FluentValidation;

namespace DispenserProvider.Models;

public class LambdaResponse
{
    public LambdaResponse(IHandlerResponse handlerResponse)
    {
        HandlerResponse = handlerResponse;
    }

    public LambdaResponse(Exception exception)
    {
        ErrorType = exception.GetType().Name;
        ErrorMessage = exception.Message;
    }

    public LambdaResponse(ValidationException exception)
    {
        var error = exception.Errors.First();
        ErrorType = error.ErrorCode;
        ErrorMessage = error.ErrorMessage;
        ErrorData = error.CustomState;
    }

    public IHandlerResponse? HandlerResponse { get; }
    public string? ErrorMessage { get; }
    public string? ErrorType { get; }
    public object? ErrorData { get; }
}