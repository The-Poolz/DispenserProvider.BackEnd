using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Validators.Signature.Models;
using DispenserProvider.Tests.Mocks.Services.Handlers.GenerateSignature.Web3;

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
                RefundFinishTime = DateTime.UtcNow.AddDays(1),
                UserAddress = "0x0000000000000000000000000000000000000001",
                WithdrawalDetail = new TransactionDetailDTO {
                    ChainId = 1,
                    PoolId = 1
                }
            };

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: true, isRefunded: false);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, true);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage("**Cannot generate signature, because asset already withdrawn.**");
        }

        [Fact]
        internal void WhenAssetAlreadyRefunded_ShouldThrowException()
        {
            var dispenser = new DispenserDTO {
                RefundFinishTime = DateTime.UtcNow.AddDays(1),
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

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: true);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, true);

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

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, true);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"**Cannot generate signature for refund, because refund time ({dispenser.RefundFinishTime}) has expired.**");
        }

        [Fact]
        internal void WhenSignatureStillValid_ShouldThrowException()
        {
            var signature = new SignatureDTO {
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow.AddDays(1)
            };
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
                },
                UserSignatures = [signature]
            };

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, true);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"**Cannot generate signature, because it is still valid until ({signature.ValidUntil}). Please try again after ({UpdatingSignatureValidator.NextTry(signature)}).**");
        }

        [Fact]
        internal void WhenGenerationOnCooldown_ShouldThrowException()
        {
            var signature = new SignatureDTO {
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidUntil = DateTime.UtcNow
            };
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
                },
                UserSignatures = [signature]
            };

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, true);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"**Cannot generate signature, because the next valid time for generation has not yet arrived. Please try again after ({UpdatingSignatureValidator.NextTry(signature)}).**");
        }

        [Fact]
        internal void WhenRequestIsValid_ShouldNotThrowException()
        {
            var signature = new SignatureDTO {
                ValidFrom = DateTime.UtcNow.AddDays(-2),
                ValidUntil = DateTime.UtcNow.AddDays(-1)
            };
            var dispenser = new DispenserDTO {
                RefundFinishTime = DateTime.UtcNow.AddDays(7),
                UserAddress = "0x0000000000000000000000000000000000000001",
                WithdrawalDetail = new TransactionDetailDTO {
                    ChainId = 1,
                    PoolId = 1
                },
                RefundDetail = new TransactionDetailDTO {
                    ChainId = 56,
                    PoolId = 1
                },
                UserSignatures = [signature]
            };

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);

            var validator = new GenerateSignatureValidator(
                updatingValidator: new UpdatingSignatureValidator(),
                refundValidator: new RefundSignatureValidator(),
                assetValidator: new AssetAvailabilityValidator(providerContract)
            );

            var request = new GenerateSignatureValidatorRequest(dispenser, true);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().NotThrow();
        }
    }
}