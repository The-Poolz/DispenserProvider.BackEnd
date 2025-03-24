using Xunit;
using FluentAssertions;
using FluentValidation;
using Net.Web3.EthereumWallet;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Services.Validators.AdminRequest;

public class UniqueAssetValidatorTests
{
    public class ValidateAndThrow
    {
        [Fact]
        internal void ShouldNotThrow_WhenAssetIsUnique()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var validator = new UniqueAssetValidator(dbFactory);
            var request = new UniqueAssetValidatorRequest(EthereumAddress.ZeroAddress, chainId: 1, poolId: 1);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().NotThrow();
        }

        [Fact]
        internal void ShouldThrow_WhenAssetIsNotUniqueForWithdrawalDetail()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var validator = new UniqueAssetValidator(dbFactory);
            var request = new UniqueAssetValidatorRequest(
                MockDispenserContext.Dispenser.UserAddress,
                MockDispenserContext.TransactionDetail.ChainId,
                MockDispenserContext.TransactionDetail.PoolId
            );

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "ASSET_MUST_BE_UNIQUE",
                    ErrorMessage = "PoolId and ChainId already used for user.",
                    CustomState = new
                    {
                        Address = MockDispenserContext.Dispenser.UserAddress,
                        MockDispenserContext.TransactionDetail.ChainId,
                        MockDispenserContext.TransactionDetail.PoolId
                    }
                });
        }

        [Fact]
        internal void ShouldThrow_WhenAssetIsNotUniqueForRefundDetail()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var refundDetail = new TransactionDetailDTO
            {
                Id = 2,
                ChainId = 1,
                PoolId = 1,
                RefundDispenser = dbFactory.Current.Dispenser.First()
            };
            dbFactory.Current.Add(refundDetail);
            dbFactory.Current.SaveChanges();
            var validator = new UniqueAssetValidator(dbFactory);
            var request = new UniqueAssetValidatorRequest(
                MockDispenserContext.Dispenser.UserAddress,
                refundDetail.ChainId,
                refundDetail.PoolId
            );

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "ASSET_MUST_BE_UNIQUE",
                    ErrorMessage = "PoolId and ChainId already used for user.",
                    CustomState = new
                    {
                        Address = MockDispenserContext.Dispenser.UserAddress,
                        refundDetail.ChainId,
                        refundDetail.PoolId
                    }
                });
        }
    }
}