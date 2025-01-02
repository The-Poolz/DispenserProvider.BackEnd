using DispenserProvider.Models;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class RetrieveSignatureResponse(Asset asset) : IHandlerResponse
{
    public RetrieveSignatureResponse() : this(default!) { }
    public RetrieveSignatureResponse(DispenserDTO dispenser, RetrieveSignatureRequest request) : this(new Asset(dispenser, request)) { }

    public Asset Asset { get; } = asset;
}