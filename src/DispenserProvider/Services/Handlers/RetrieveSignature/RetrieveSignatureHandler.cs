using DispenserProvider.DataBase;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature;

public class RetrieveSignatureHandler(DispenserContext dispenserContext) : IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>
{
    public RetrieveSignatureResponse Handle(RetrieveSignatureRequest request)
    {


        throw new NotImplementedException($"The 'Handle' method is not implemented, in '{nameof(RetrieveSignatureHandler)}' class.");
    }
}