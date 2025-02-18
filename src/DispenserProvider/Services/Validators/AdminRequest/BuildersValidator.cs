using FluentValidation;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Services.Web3.Contracts;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class BuildersValidator : AbstractValidator<BuildersValidatorRequest>
{
    public BuildersValidator(ILockDealNFTContract lockDealNFT, IBuilderContract builderContract)
        : this(
            new BuilderValidator(lockDealNFT, builderContract, "DealProvider", "LockDealProvider", "TimedDealProvider"),
            new BuilderValidator(lockDealNFT, builderContract, "DealProvider")
        )
    { }

    public BuildersValidator(IValidator<ChainAddressPair> refundValidator, IValidator<ChainAddressPair> withdrawValidator)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleForEach(x => x.Withdraw)
            .SetValidator(withdrawValidator);

        RuleFor(x => x.Refund)
            .SetValidator(refundValidator!)
            .When(x => x.Refund != null);
    }
}