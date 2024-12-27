using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.GenerateSignature;
using DispenserProvider.Tests.Mocks.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Tests.Services.Validators.GenerateSignature;

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

            var providerContract = MockDispenserProviderContract.Create(dispenser, isWithdrawn: false, isRefunded: true);

            var validator = new AssetAvailabilityValidator(providerContract);

            var testCode = () => validator.ValidateAndThrow(dispenser);

            testCode.Should().Throw<ValidationException>()
                .WithMessage("**Cannot generate signature, because asset already refunded.**");
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