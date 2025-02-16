using Xunit;
using FluentAssertions;
using FluentValidation;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Tests.Mocks.Services.Web3;
using DispenserProvider.Services.Validators.Signature;

namespace DispenserProvider.Tests.Services.Validators.Generate;

public class AssetAvailabilityValidatorTests
{
    public class ValidateAndThrow
    {
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

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: true, isRefunded: false);

            var validator = new AssetAvailabilityValidator(providerContract);

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"*{ErrorCode.ASSET_ALREADY_WITHDRAWN.ToErrorMessage()}*");
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

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: true);

            var validator = new AssetAvailabilityValidator(providerContract);

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"*{ErrorCode.ASSET_ALREADY_REFUNDED.ToErrorMessage()}*");
        }

        [Fact]
        internal void WhenAssetNotTaken_ShouldNotThrowException()
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

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: false);

            var validator = new AssetAvailabilityValidator(providerContract);

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().NotThrow();
        }
    }
}