using MediatR;
using FluentValidation;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Validators.Signature.Models;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature;

public class RetrieveSignatureHandler(
    IDispenserManager dispenserManager,
    IValidator<RetrieveSignatureValidatorRequest> retrieveValidator
)
    : IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>
{
    public Task<RetrieveSignatureResponse> Handle(RetrieveSignatureRequest request, CancellationToken cancellationToken)
    {
        var dispenser = dispenserManager.GetDispenser(request);
        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == request.ChainId && dispenser.RefundDetail.PoolId == request.PoolId;

        retrieveValidator.ValidateAndThrow(new RetrieveSignatureValidatorRequest(dispenser, isRefund));

        var transactionDetail = isRefund ? dispenser.RefundDetail! : dispenser.WithdrawalDetail;
        return Task.FromResult(new RetrieveSignatureResponse(new Asset(dispenser, transactionDetail, isRefund)));
    }
}