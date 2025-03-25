using FluentValidation;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class UniqueAssetValidator : AbstractValidator<UniqueAssetValidatorRequest>
{
    public UniqueAssetValidator(IDispenserManager dispenserManager)
    {
        RuleFor(x => x)
            .Must(x => !dispenserManager.GetDispensers(x.Users, x.ChainId, x.PoolId).Any())
            .WithError(ErrorCode.ASSET_MUST_BE_UNIQUE, x => dispenserManager.GetDispensers(x.Users, x.ChainId, x.PoolId));
    }
}
