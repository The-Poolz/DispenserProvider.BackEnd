using FluentValidation;

namespace DispenserProvider.Models;

public class LambdaResponse
{
    public LambdaResponse(IHandlerResponse data)
    {
        Data = data;
    }

    public LambdaResponse(Exception exception)
    {
        Error = new LambdaError(exception);
    }

    public LambdaResponse(ValidationException exception)
    {
        Error = new LambdaError(exception);
    }

    public IHandlerResponse? Data { get; }
    public LambdaError? Error { get; }
}