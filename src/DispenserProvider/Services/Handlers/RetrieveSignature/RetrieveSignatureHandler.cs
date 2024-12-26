using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature;

public class RetrieveSignatureHandler : IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>
{
    public RetrieveSignatureResponse Handle(RetrieveSignatureRequest request)
    {
        throw new NotImplementedException($"The 'Handle' method is not implemented, in '{nameof(RetrieveSignatureHandler)}' class.");
    }
}