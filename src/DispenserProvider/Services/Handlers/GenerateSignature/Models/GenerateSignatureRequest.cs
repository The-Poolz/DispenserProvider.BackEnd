using MediatR;
using Net.Web3.EthereumWallet;
using DispenserProvider.Models;
using DispenserProvider.Services.Database;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Models;

public class GenerateSignatureRequest : SignatureRequest, IRequest<GenerateSignatureResponse>
{
    internal GenerateSignatureRequest(long chainId, long poolId, EthereumAddress userAddress)
    {
        ChainId = chainId;
        PoolId = poolId;
        UserAddress = userAddress;
        ValidatorRequest = null!;
    }

    public GenerateSignatureRequest(SignatureRequest request, IServiceProvider serviceProvider)
        : this(request.ChainId, request.PoolId, request.UserAddress)
    {
        var dispenserManager = serviceProvider.GetRequiredService<IDispenserManager>();
        var dispenser = dispenserManager.GetDispenser(this);
        var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == ChainId && dispenser.RefundDetail.PoolId == PoolId;
        ValidatorRequest = new GenerateSignatureValidatorRequest(dispenser, isRefund);
    }

    public GenerateSignatureValidatorRequest ValidatorRequest { get; }
}
