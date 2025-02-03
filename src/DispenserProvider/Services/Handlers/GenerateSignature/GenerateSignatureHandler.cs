using FluentValidation;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Validators.Signature.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature;

public class GenerateSignatureHandler(
    IDispenserManager dispenserManager,
    ISignatureProcessor signatureProcessor,
    IValidator<GenerateSignatureValidatorRequest> validator
)
    : IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>
{
    public GenerateSignatureResponse Handle(GenerateSignatureRequest request)
    {
        var dispenser = dispenserManager.GetDispenser(request);

        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == request.ChainId && dispenser.RefundDetail.PoolId == request.PoolId;

        validator.ValidateAndThrow(new GenerateSignatureValidatorRequest(dispenser, isRefund));

        var validFrom = signatureProcessor.SaveSignature(dispenser, isRefund);

        return new GenerateSignatureResponse(validFrom);
    }
}