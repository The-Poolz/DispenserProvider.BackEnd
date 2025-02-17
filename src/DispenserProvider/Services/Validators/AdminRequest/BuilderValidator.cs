using FluentValidation;
using DispenserProvider.Services.Web3;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class BuilderValidator : AbstractValidator<ChainAddressPair>
{
    private readonly string[] _validProviderNames = ["DealProvider", "LockDealProvider", "TimedDealProvider"];

    public BuilderValidator(ILockDealNFTContract lockDealNFT, IBuilderContract builderContract)
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .Must(x => lockDealNFT.ApprovedContract(x.ChainId, x.Address))
            .WithError(ErrorCode.BUILDER_MUST_BE_APPROVED_IN_LOCK_DEAL_NFT, x => new
            {
                x.ChainId,
                x.Address
            })
            .Must(x => _validProviderNames.Contains(builderContract.Name(x.ChainId, x.Address)))
            .WithError(ErrorCode.BUILDER_MUST_BE_SIMPLE_PROVIDER, x => new
            {
                x.ChainId,
                x.Address
            });
    }
}