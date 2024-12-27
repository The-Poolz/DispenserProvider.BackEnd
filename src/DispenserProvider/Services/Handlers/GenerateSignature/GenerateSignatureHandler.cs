using FluentValidation;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Helpers;
using DispenserProvider.Services.Validators.GenerateSignature.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature;

public class GenerateSignatureHandler(
    DispenserContext dispenserContext,
    ISignatureProcessor signatureProcessor,
    IValidator<GenerateSignatureValidatorRequest> validator
)
    : IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>
{
    public GenerateSignatureResponse Handle(GenerateSignatureRequest request)
    {
        var dispenser = dispenserContext.Dispenser
            .Include(x => x.UserSignatures)
            .Include(x => x.WithdrawalDetail)
                .ThenInclude(x => x.Builders)
            .Include(x => x.RefundDetail)
                .ThenInclude(x => x!.Builders)
            .FirstOrDefault(x =>
                x.DeletionLogSignature == null &&
                x.UserAddress == request.UserAddress.Address &&
                ((x.WithdrawalDetail.ChainId == request.ChainId && x.WithdrawalDetail.PoolId == request.PoolId) ||
                 (x.RefundDetail != null && x.RefundDetail.ChainId == request.ChainId && x.RefundDetail.PoolId == request.PoolId))
            ) ?? throw new InvalidOperationException($"Asset by provided PoolId={request.PoolId} and ChainId={request.ChainId} for '{request.UserAddress}' user, not found.");

        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == request.ChainId && dispenser.RefundDetail.PoolId == request.PoolId;

        validator.ValidateAndThrow(new GenerateSignatureValidatorRequest(dispenser, isRefund));

        var validFrom = signatureProcessor.SaveSignature(dispenser, isRefund);

        return new GenerateSignatureResponse(validFrom);
    }
}