﻿using FluentValidation;
using DispenserProvider.Extensions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Services.Validators.Signature;

public class AssetAvailabilityValidator : AbstractValidator<DispenserDTO>
{
    public AssetAvailabilityValidator(IDispenserProviderContract dispenserContract)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .Must(x => !dispenserContract.IsTaken(x.WithdrawalDetail.ChainId, x.WithdrawalDetail.PoolId, x.UserAddress))
            .WithErrorCode(ErrorCode.ASSET_ALREADY_WITHDRAWN.ToErrorMessage())
            .WithMessage(ErrorCode.ASSET_ALREADY_WITHDRAWN.ToErrorMessage());

        RuleFor(x => x)
            .Must(x => !dispenserContract.IsTaken(x.RefundDetail!.ChainId, x.RefundDetail.PoolId, x.UserAddress))
            .When(x => x.RefundDetail != null)
            .WithErrorCode(ErrorCode.ASSET_ALREADY_REFUNDED.ToErrorCode())
            .WithMessage(ErrorCode.ASSET_ALREADY_REFUNDED.ToErrorMessage());
    }
}