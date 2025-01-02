using DispenserProvider.Models;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class RetrieveSignatureResponse(DispenserDTO dispenser, RetrieveSignatureRequest request) : IHandlerResponse
{
    public RetrieveSignatureResponse() : this(default!, default!) { }

    public Asset Asset { get; } = new(dispenser, request);
}