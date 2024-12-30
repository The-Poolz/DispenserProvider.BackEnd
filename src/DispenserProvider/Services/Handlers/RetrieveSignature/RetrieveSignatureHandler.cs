using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature;

public class RetrieveSignatureHandler(
    IDispenserManager dispenserManager,
    IDispenserProviderContract dispenserContract
)
    : IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>
{
    public RetrieveSignatureResponse Handle(RetrieveSignatureRequest request)
    {
        var dispenser = dispenserManager.GetDispenser(request);

        var isTaken = dispenserContract.IsTaken();

        throw new NotImplementedException($"The 'Handle' method is not implemented, in '{nameof(RetrieveSignatureHandler)}' class.");
    }
}