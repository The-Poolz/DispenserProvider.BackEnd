using FluentValidation;
using DispenserProvider.Services.Web3;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class PoolOwnershipValidator : AbstractValidator<PoolOwnershipValidatorRequest>
{
	public PoolOwnershipValidator(IChainProvider chainProvider, ILockDealNFTContract lockDealNFT)
    {
        RuleFor(x => x)
            .Must(x => chainProvider.DispenserProviderContract(x.ChainId) == lockDealNFT.OwnerOf(x.ChainId, x.PoolId))
            .WithErrorCode();
    }
}
