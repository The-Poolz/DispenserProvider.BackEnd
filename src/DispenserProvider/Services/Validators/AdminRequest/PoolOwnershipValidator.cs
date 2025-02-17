using FluentValidation;
using DispenserProvider.Services.Web3;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Services.Web3.Contracts;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class PoolOwnershipValidator : AbstractValidator<PoolOwnershipValidatorRequest>
{
	public PoolOwnershipValidator(ISignerManager signerManager, ILockDealNFTContract lockDealNFT)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Withdraw)
            .Must(x => signerManager.GetSigner().GetPublicAddress() == lockDealNFT.OwnerOf(x.ChainId, x.PoolId))
            .WithError(ErrorCode.INVALID_TOKEN_OWNER, x => new
            {
                x.Withdraw.ChainId,
                x.Withdraw.PoolId
            });

        RuleFor(x => x.Refund!)
            .Must(x => Equals(
                signerManager.GetSigner().GetPublicAddress(),
                lockDealNFT.OwnerOf(x.ChainId, x.PoolId).Address
            ))
            .When(x => x.Refund != null)
            .WithError(ErrorCode.INVALID_TOKEN_OWNER, x => new
            {
                x.Refund!.ChainId,
                x.Refund!.PoolId
            });
    }
}