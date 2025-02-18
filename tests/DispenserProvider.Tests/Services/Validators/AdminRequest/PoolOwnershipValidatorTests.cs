using Xunit;
using FluentAssertions;
using FluentValidation;
using Net.Web3.EthereumWallet;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Tests.Mocks.Services.Web3;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Services.Validators.AdminRequest;

public class PoolOwnershipValidatorTests
{
    public class ValidateAndThrow
    {
        [Fact]
        internal void WhenWithdrawnOwnerInvalid_ShouldThrowException()
        {
            var withdraw = new ChainPoolPair(1, 1);
            var request = new PoolOwnershipValidatorRequest(withdraw);

            var validator = new PoolOwnershipValidator(
                new MockSignerManager(MockUsers.Admin.PrivateKey),
                new MockLockDealNFTContractBuilder()
                    .WithOwnerOf(withdraw.ChainId, withdraw.PoolId, EthereumAddress.ZeroAddress)
                    .Build()
            );

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "INVALID_TOKEN_OWNER",
                    ErrorMessage = "Owner of provided PoolId in the provided ChainId is invalid.",
                    CustomState = new
                    {
                        withdraw.ChainId,
                        withdraw.PoolId
                    }
                });
        }

        [Fact]
        internal void WhenRefundOwnerInvalid_ShouldThrowException()
        {
            var withdraw = new ChainPoolPair(1, 1);
            var refund = new ChainPoolPair(56, 1);
            var request = new PoolOwnershipValidatorRequest(withdraw, refund);

            var validator = new PoolOwnershipValidator(
                new MockSignerManager(MockUsers.Admin.PrivateKey),
                new MockLockDealNFTContractBuilder()
                    .WithOwnerOf(withdraw.ChainId, withdraw.PoolId, MockUsers.Admin.Address)
                    .WithOwnerOf(refund.ChainId, refund.PoolId, EthereumAddress.ZeroAddress)
                    .Build()
            );

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "INVALID_TOKEN_OWNER",
                    ErrorMessage = "Owner of provided PoolId in the provided ChainId is invalid.",
                    CustomState = new
                    {
                        refund.ChainId,
                        refund.PoolId
                    }
                });
        }

        [Fact]
        internal void WhenOwnerIsValid_ShouldWithoutException()
        {
            var withdraw = new ChainPoolPair(1, 1);
            var refund = new ChainPoolPair(56, 1);
            var request = new PoolOwnershipValidatorRequest(withdraw, refund);

            var validator = new PoolOwnershipValidator(
                new MockSignerManager(MockUsers.Admin.PrivateKey),
                new MockLockDealNFTContractBuilder()
                    .WithOwnerOf(withdraw.ChainId, withdraw.PoolId, MockUsers.Admin.Address)
                    .WithOwnerOf(refund.ChainId, refund.PoolId, MockUsers.Admin.Address)
                    .Build()
            );

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().NotThrow<ValidationException>();
        }
    }
}