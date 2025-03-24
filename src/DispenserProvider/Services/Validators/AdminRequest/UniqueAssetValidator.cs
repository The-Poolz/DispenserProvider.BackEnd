using FluentValidation;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Validators.AdminRequest;

public class UniqueAssetValidator : AbstractValidator<UniqueAssetValidatorRequest>
{
    public UniqueAssetValidator(IDbContextFactory<DispenserContext> dispenserContextFactory)
    {
        RuleFor(x => x)
            .Must(x =>
            {
                using var context = dispenserContextFactory.CreateDbContext();
                return !context.Dispenser
                    .Any(d =>
                        d.UserAddress == x.Address &&
                        ((d.WithdrawalDetail.ChainId == x.ChainId && d.WithdrawalDetail.PoolId == x.PoolId) ||
                         (d.RefundDetail != null && d.RefundDetail.ChainId == x.ChainId && d.RefundDetail.PoolId == x.PoolId))
                    );
            })
            .WithError(ErrorCode.ASSET_MUST_BE_UNIQUE, x => x);
    }
}