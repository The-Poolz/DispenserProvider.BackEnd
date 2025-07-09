using FluentValidation;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Handlers.AdminListAssets.Models;

namespace DispenserProvider.Services.Handlers.AdminListAssets;

public class AdminListAssetsValidator : AbstractValidator<AdminListAssetsRequest>
{
    public const int MaxPageSize = 1000;

    public AdminListAssetsValidator()
    {
        RuleFor(x => x.Limit)
            .Must(x => x > 0)
            .WithError(ErrorCode.LIMIT_MUST_BE_MORE_THEN_ZERO)
            .Must(x => x <= MaxPageSize)
            .WithError(ErrorCode.LIMIT_MUST_BE_LOWER_THEN_MAX_ALLOWED_VALUE, x => new
            {
                MaxLimitValue = MaxPageSize
            });

        RuleFor(x => x.Page)
            .Must(x => x > 0)
            .WithError(ErrorCode.LIMIT_MUST_BE_MORE_THEN_ZERO);
    }
}