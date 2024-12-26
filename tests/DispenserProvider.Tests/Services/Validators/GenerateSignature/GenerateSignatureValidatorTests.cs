using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.GenerateSignature;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Validators.GenerateSignature.Models;

namespace DispenserProvider.Tests.Services.Validators.GenerateSignature;

public class GenerateSignatureValidatorTests
{
    public class ValidateAndThrow
    {
        public ValidateAndThrow()
        {
            Environment.SetEnvironmentVariable("COOLDOWN_OFFSET_IN_SECONDS", "300");
        }

        [Fact]
        internal void WhenAssetAlreadyWithdrawn_ShouldThrowException()
        {
            var dispenser = new DispenserDTO {
                UserAddress = "0x0000000000000000000000000000000000000001",
                WithdrawalDetail = new TransactionDetailDTO {
                    ChainId = 1,
                    PoolId = 1
                }
            };

            var providerContract = new Mock<IDispenserProviderContract>();
            providerContract.Setup(x => x.IsTaken(dispenser.WithdrawalDetail.ChainId, dispenser.WithdrawalDetail.PoolId, dispenser.UserAddress))
                .Returns(true);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract.Object)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, false);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage("**Cannot generate signature, because asset already withdrawn.**");
        }

        [Fact]
        internal void WhenAssetAlreadyRefunded_ShouldThrowException()
        {
            var dispenser = new DispenserDTO {
                UserAddress = "0x0000000000000000000000000000000000000001",
                WithdrawalDetail = new TransactionDetailDTO {
                    ChainId = 1,
                    PoolId = 1
                },
                RefundDetail = new TransactionDetailDTO {
                    ChainId = 56,
                    PoolId = 1
                }
            };

            var providerContract = new Mock<IDispenserProviderContract>();
            providerContract.Setup(x => x.IsTaken(dispenser.WithdrawalDetail.ChainId, dispenser.WithdrawalDetail.PoolId, dispenser.UserAddress))
                .Returns(false);
            providerContract.Setup(x => x.IsTaken(dispenser.RefundDetail.ChainId, dispenser.RefundDetail.PoolId, dispenser.UserAddress))
                .Returns(true);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract.Object)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, false);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage("**Cannot generate signature, because asset already refunded.**");
        }

        [Fact]
        internal void WhenRefundTimeIsExpired_ShouldThrowException()
        {
            var dispenser = new DispenserDTO {
                RefundFinishTime = DateTime.UtcNow.AddDays(-1),
                UserAddress = "0x0000000000000000000000000000000000000001",
                WithdrawalDetail = new TransactionDetailDTO {
                    ChainId = 1,
                    PoolId = 1
                },
                RefundDetail = new TransactionDetailDTO {
                    ChainId = 56,
                    PoolId = 1
                }
            };

            var providerContract = new Mock<IDispenserProviderContract>();
            providerContract.Setup(x => x.IsTaken(dispenser.WithdrawalDetail.ChainId, dispenser.WithdrawalDetail.PoolId, dispenser.UserAddress))
                .Returns(false);
            providerContract.Setup(x => x.IsTaken(dispenser.RefundDetail.ChainId, dispenser.RefundDetail.PoolId, dispenser.UserAddress))
                .Returns(false);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract.Object)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, true);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"**Cannot generate signature for refund, because refund time ({dispenser.RefundFinishTime}) has expired.**");
        }

        [Fact]
        internal void WhenSignatureStillValid_ShouldThrowException()
        {
            var signature = new SignatureDTO
            {
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow.AddDays(1)
            };
            var dispenser = new DispenserDTO
            {
                UserSignatures = [signature]
            };

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"**Cannot generate signature, because it is still valid until ({signature.ValidUntil}). Please try again after ({NextTry(signature)}).**");
        }

        [Fact]
        internal void WhenGenerationOnCooldown_ShouldThrowException()
        {
            var signature = new SignatureDTO
            {
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow
            };
            var dispenser = new DispenserDTO
            {
                UserSignatures = [signature]
            };

            var validator = new UpdatingSignatureValidator();

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"**Cannot generate signature, because the next valid time for generation has not yet arrived. Please try again after ({NextTry(signature)}).**");
        }
    }
}