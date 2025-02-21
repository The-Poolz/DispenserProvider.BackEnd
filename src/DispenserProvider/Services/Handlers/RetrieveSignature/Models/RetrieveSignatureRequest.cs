using DispenserProvider.Models;
using DispenserProvider.Services.Database;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class RetrieveSignatureRequest : SignatureRequest, IHandlerRequest<RetrieveSignatureResponse>
{
    public RetrieveSignatureRequest(SignatureRequest signatureRequest, IServiceProvider serviceProvider)
    {
        ChainId = signatureRequest.ChainId;
        PoolId = signatureRequest.PoolId;
        UserAddress = signatureRequest.UserAddress;

        var dispenserManager = serviceProvider.GetRequiredService<IDispenserManager>();
        var dispenser = dispenserManager.GetDispenser(this);
        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == ChainId && dispenser.RefundDetail.PoolId == PoolId;
        ValidatorRequest = new RetrieveSignatureValidatorRequest(dispenser, isRefund);
    }

    public RetrieveSignatureValidatorRequest ValidatorRequest { get; }
}