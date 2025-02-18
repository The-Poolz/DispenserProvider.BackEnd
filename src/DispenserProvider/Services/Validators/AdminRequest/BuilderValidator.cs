using FluentValidation;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Web3.Contracts;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class BuilderValidator : AbstractValidator<ChainAddressPair>
{
    public BuilderValidator(ILockDealNFTContract lockDealNFT, IBuilderContract builderContract, params string[] validProviderNames)
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .Must(x => lockDealNFT.ApprovedContract(x.ChainId, x.Address))
            .WithError(ErrorCode.BUILDER_MUST_BE_APPROVED_IN_LOCK_DEAL_NFT, x => x)
            .Must(x => validProviderNames.Contains(builderContract.Name(x.ChainId, x.Address)))
            .WithError(ErrorCode.BUILDER_MUST_BE_SIMPLE_PROVIDER, x => x);
    }
}