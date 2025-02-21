using MediatR;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature;

public class GenerateSignatureHandler(IDispenserManager dispenserManager, ISignatureProcessor signatureProcessor)
    : IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>
{
    public Task<GenerateSignatureResponse> Handle(GenerateSignatureRequest request, CancellationToken cancellationToken)
    {
        var dispenser = dispenserManager.GetDispenser(request);
        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == request.ChainId && dispenser.RefundDetail.PoolId == request.PoolId;
        var validFrom = signatureProcessor.SaveSignature(dispenser, isRefund);
        return Task.FromResult(new GenerateSignatureResponse(validFrom));
    }
}