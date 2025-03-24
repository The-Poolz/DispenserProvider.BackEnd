using FluentValidation;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.MessageTemplate.Models.Validators;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset;

public class CreateAssetValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetValidator(
        IValidator<CreateValidatorSettings> requestValidator,
        IValidator<PoolOwnershipValidatorRequest> poolOwnershipValidator,
        IValidator<BuildersValidatorRequest> buildersValidator,
        IValidator<UniqueAssetValidatorRequest> uniqueAssetValidator
    )
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Message)
            .Must(x => x.ChainId != x.Refund!.ChainId || x.PoolId != x.Refund!.PoolId)
            .When(x => x.Message.Refund != null)
            .WithError(ErrorCode.POOL_ID_DUPLICATION);

        RuleFor(x => new CreateValidatorSettings(
            new AdminRequestValidatorSettings(x.Signature, x.Message.Eip712Message),
            x.Message.UsersToValidate,
            x.Message.ScheduleToValidate
        )).SetValidator(requestValidator);

        RuleFor(x => new PoolOwnershipValidatorRequest(
            new ChainPoolPair(x.Message.ChainId, x.Message.PoolId),
            x.Message.Refund != null ? new ChainPoolPair(x.Message.Refund.ChainId, x.Message.Refund.PoolId) : null
        )).SetValidator(poolOwnershipValidator);

        RuleFor(x => new BuildersValidatorRequest(
            x.Message.Schedules.Select(s => new ChainAddressPair(x.Message.ChainId, s.ProviderAddress)),
            x.Message.Refund != null ? new ChainAddressPair(x.Message.Refund.ChainId, x.Message.Refund.DealProvider) : null
        )).SetValidator(buildersValidator);

        RuleForEach(x => x.Message.Users.Select(u => new UniqueAssetValidatorRequest(u.UserAddress, x.Message.ChainId, x.Message.PoolId)))
            .SetValidator(uniqueAssetValidator);

        RuleForEach(x => x.Message.Users.Select(u => new UniqueAssetValidatorRequest(u.UserAddress, x.Message.Refund!.ChainId, x.Message.Refund!.PoolId)))
            .SetValidator(uniqueAssetValidator)
            .When(x => x.Message.Refund != null);
    }
}