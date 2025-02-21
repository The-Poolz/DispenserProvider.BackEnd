using FluentValidation;
using DispenserProvider.Services.Validators.Signature.Models;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature;

public class RetrieveSignatureValidator : AbstractValidator<RetrieveSignatureRequest>
{
    public RetrieveSignatureValidator(IValidator<RetrieveSignatureValidatorRequest> retrieveValidator)
    {
        RuleFor(x => x.ValidatorRequest)
            .SetValidator(retrieveValidator);
    }
}