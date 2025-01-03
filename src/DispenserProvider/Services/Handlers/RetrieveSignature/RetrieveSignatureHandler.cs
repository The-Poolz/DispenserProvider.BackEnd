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
    public RetrieveSignatureResponse Handle(RetrieveSignatureRequest request)
    {
        var dispenser = dispenserManager.GetDispenser(request);
        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == request.ChainId && dispenser.RefundDetail.PoolId == request.PoolId;

        retrieveValidator.ValidateAndThrow(new RetrieveSignatureValidatorRequest(dispenser, isRefund));

        var transactionDetail = isRefund ? dispenser.RefundDetail! : dispenser.WithdrawalDetail;
        return new RetrieveSignatureResponse(new Asset(dispenser, transactionDetail, isRefund));
    }
}