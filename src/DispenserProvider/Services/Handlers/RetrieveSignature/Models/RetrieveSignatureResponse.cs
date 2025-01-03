using DispenserProvider.Models;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class RetrieveSignatureResponse(Asset asset) : IHandlerResponse
{
    public RetrieveSignatureResponse() : this(default!) { }
    public RetrieveSignatureResponse(DispenserDTO dispenser, bool isRefund) : this(new Asset(dispenser, isRefund)) { }

    public Asset Asset { get; } = asset;
}