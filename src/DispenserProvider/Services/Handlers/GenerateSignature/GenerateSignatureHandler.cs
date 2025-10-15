using MediatR;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Extensions;
using Net.Utils.ErrorHandler.Extensions;

namespace DispenserProvider.Services.Handlers.GenerateSignature;

public class GenerateSignatureHandler(IDispenserManager dispenserManager, ISignatureProcessor signatureProcessor)
    : IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>
{
    public Task<GenerateSignatureResponse> Handle(GenerateSignatureRequest request, CancellationToken cancellationToken)
    {
        if (TestnetChainsManager.TestnetChains.Contains(request.ChainId))
        {
            throw ErrorCode.TESTNET_OUT_OF_SUPPORT.ToException(new
            {
                request.ChainId
            });
        }
        var dispenser = dispenserManager.GetDispenser(request);
        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == request.ChainId && dispenser.RefundDetail.PoolId == request.PoolId;
        var validFrom = signatureProcessor.SaveSignature(dispenser, isRefund);
        return Task.FromResult(new GenerateSignatureResponse(validFrom));
    }
}