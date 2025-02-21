using MediatR;

namespace DispenserProvider.Models;

public interface IHandlerRequest<out TResponse> : IRequest<TResponse> where TResponse : IHandlerResponse;