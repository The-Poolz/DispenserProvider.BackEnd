using FluentValidation;
using DispenserProvider.MessageTemplate.Models.Validators;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;

namespace DispenserProvider.Services.Handlers.DeleteAsset;

public class DeleteAssetValidator : AbstractValidator<DeleteAssetRequest>
{
    public DeleteAssetValidator(IValidator<DeleteValidatorSettings> requestValidator)
    {
        RuleFor(x => new DeleteValidatorSettings(
            new AdminRequestValidatorSettings(x.Signature, x.Message.Eip712Message),
            x.Message.UsersToValidate
        )).SetValidator(requestValidator);
    }
}