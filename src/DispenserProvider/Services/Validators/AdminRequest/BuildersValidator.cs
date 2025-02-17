using FluentValidation;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class BuildersValidator : AbstractValidator<BuildersValidatorRequest>
{
    public BuildersValidator(IValidator<ChainAddressPair> builderValidator)
    {
        RuleForEach(x => x.Withdraw)
            .SetValidator(builderValidator);

        RuleFor(x => x.Refund)
            .SetValidator(builderValidator!)
            .When(x => x.Refund != null);
    }
}