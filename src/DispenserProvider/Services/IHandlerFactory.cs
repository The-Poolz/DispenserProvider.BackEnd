using DispenserProvider.Models;

namespace DispenserProvider.Services;

public interface IHandlerFactory
{
    IHandlerResponse Handle(LambdaRequest request);
}