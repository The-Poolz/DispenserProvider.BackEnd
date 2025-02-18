using FluentValidation;
using DispenserProvider.DataBase.Models;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Web3.Contracts;

namespace DispenserProvider.Services.Validators.Signature;

public class AssetAvailabilityValidator : AbstractValidator<DispenserDTO>
{
    public AssetAvailabilityValidator(IDispenserProviderContract dispenserContract)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .Must(x => !dispenserContract.IsTaken(x.WithdrawalDetail.ChainId, x.WithdrawalDetail.PoolId, x.UserAddress))
            .WithError(ErrorCode.ASSET_ALREADY_WITHDRAWN);

        RuleFor(x => x)
            .Must(x => !dispenserContract.IsTaken(x.RefundDetail!.ChainId, x.RefundDetail.PoolId, x.UserAddress))
            .When(x => x.RefundDetail != null)
            .WithError(ErrorCode.ASSET_ALREADY_REFUNDED);
    }
}