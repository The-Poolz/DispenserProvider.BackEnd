using MediatR;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature;

public class RetrieveSignatureHandler(IDispenserManager dispenserManager) : IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>
{
    public Task<RetrieveSignatureResponse> Handle(RetrieveSignatureRequest request, CancellationToken cancellationToken)
    {
        request.InitializeValidatorRequest(dispenserManager);
        var dispenser = dispenserManager.GetDispenser(request);
        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == request.ChainId && dispenser.RefundDetail.PoolId == request.PoolId;
        var transactionDetail = isRefund ? dispenser.RefundDetail! : dispenser.WithdrawalDetail;
        return Task.FromResult(new RetrieveSignatureResponse(new Asset(dispenser, transactionDetail, isRefund)));
    }
}