using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature;

public class RetrieveSignatureHandler(
    IDispenserManager dispenserManager,
    IValidator<DispenserDTO> retrieveValidator
)
    : IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>
{
    public RetrieveSignatureResponse Handle(RetrieveSignatureRequest request)
    {
        var dispenser = dispenserManager.GetDispenser(request);

        retrieveValidator.ValidateAndThrow(dispenser);

        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == request.ChainId && dispenser.RefundDetail.PoolId == request.PoolId;
        var transactionDetail = isRefund ? dispenser.RefundDetail! : dispenser.WithdrawalDetail;

        return new RetrieveSignatureResponse(new Asset(dispenser, transactionDetail, isRefund));
    }
}