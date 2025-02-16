using Xunit;
using FluentAssertions;
using FluentValidation;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Tests.Mocks.Services.Web3;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Tests.Services.Validators.Generate;

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
                .WithMessage($"*{ErrorCode.ASSET_ALREADY_WITHDRAWN.ToErrorMessage()}*");
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
                .WithMessage($"*{ErrorCode.ASSET_ALREADY_REFUNDED.ToErrorMessage()}*");
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
                .WithMessage($"*{ErrorCode.REFUND_TIME_IS_EXPIRED.ToErrorMessage()}*");
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
                .WithMessage($"*{ErrorCode.SIGNATURE_IS_STILL_VALID.ToErrorMessage()}*");
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
                .WithMessage($"*{ErrorCode.SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED.ToErrorMessage()}*");
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