using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Services.Validators.Signature;

public class AssetAvailabilityValidator : AbstractValidator<DispenserDTO>
{
    public const string ErrorCodeWithdrawn = "withdrawn";
    public const string ErrorCodeRefunded = "refunded";

    public AssetAvailabilityValidator(IDispenserProviderContract dispenserContract)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .Must(x => !dispenserContract.IsTaken(x.WithdrawalDetail.ChainId, x.WithdrawalDetail.PoolId, x.UserAddress))
            .WithMessage(ErrorMessage(ErrorCodeWithdrawn))
            .WithErrorCode(ErrorCodeWithdrawn);

        RuleFor(x => x)
            .Must(x => !dispenserContract.IsTaken(x.RefundDetail!.ChainId, x.RefundDetail.PoolId, x.UserAddress))
            .When(x => x.RefundDetail != null)
            .WithMessage(ErrorMessage(ErrorCodeRefunded))
            .WithErrorCode(ErrorCodeRefunded);
    }

    private static string ErrorMessage(string status) => $"Cannot generate signature, because asset already {status}.";
}