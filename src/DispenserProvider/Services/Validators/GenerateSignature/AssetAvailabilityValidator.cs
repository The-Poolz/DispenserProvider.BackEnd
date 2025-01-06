using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Services.Validators.GenerateSignature;

public class AssetAvailabilityValidator : AbstractValidator<DispenserDTO>
{
    public const string Withdrawn = "withdrawn";
    public const string Refunded = "refunded";

    public AssetAvailabilityValidator(IDispenserProviderContract dispenserContract)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .Must(x => !dispenserContract.IsTaken(x.WithdrawalDetail.ChainId, x.WithdrawalDetail.PoolId, x.UserAddress))
            .WithMessage(ErrorMessage(Withdrawn))
            .WithErrorCode(Withdrawn);

        RuleFor(x => x)
            .Must(x => !dispenserContract.IsTaken(x.RefundDetail!.ChainId, x.RefundDetail.PoolId, x.UserAddress))
            .When(x => x.RefundDetail != null)
            .WithMessage(ErrorMessage(Refunded))
            .WithErrorCode(Refunded);
    }

    private static string ErrorMessage(string status) => $"Cannot generate signature, because asset already {status}.";
}