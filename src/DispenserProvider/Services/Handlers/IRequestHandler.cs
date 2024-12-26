using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers;

public interface IRequestHandler<in TRequest, out TResponse>
    where TRequest : IHandlerRequest, new()
    where TResponse : IHandlerResponse, new()
{
    public TResponse Handle(TRequest request);
}