namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class RetrieveSignatureResponse(Asset asset)
{
    public RetrieveSignatureResponse() : this(default!) { }

    public Asset Asset { get; } = asset;
}