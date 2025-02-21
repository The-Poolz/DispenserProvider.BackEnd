using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class RetrieveSignatureResponse(Asset asset) : IHandlerResponse
{
    public RetrieveSignatureResponse() : this(default!) { }

    public Asset Asset { get; } = asset;
}