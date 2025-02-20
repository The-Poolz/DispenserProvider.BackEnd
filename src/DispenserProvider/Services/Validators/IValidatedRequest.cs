using MediatR;

namespace DispenserProvider.Services.Validators;

public interface IValidatedRequest<TRequest> : IRequest<TRequest>
    where TRequest : IRequest<TRequest>
{
    protected Lazy<TRequest> LazyValidatorRequest { get; }
    public TRequest ValidatorRequest => LazyValidatorRequest.Value;

    protected TRequest ValidatorRequestFactory(Func<TRequest> valueFactory);
}