using Xunit;
using FluentAssertions;
using FluentValidation;
using Net.Web3.EthereumWallet;
using DispenserProvider.Tests.Mocks.Services.Web3;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Services.Validators.AdminRequest;

public class BuilderValidatorTests
{
    public class ValidateAndThrow
    {
        [Fact]
        internal void WhenAddressNotApproved_ShouldThrowException()
        {
            var withdraw = new ChainAddressPair(97, EthereumAddress.ZeroAddress);

            var validator = new BuilderValidator(
                new MockLockDealNFTContractBuilder()
                    .WithApprovedContract(withdraw.ChainId, withdraw.Address, false)
                    .Build(),
                new MockBuilderContract(isConfigured: false),
                "DealProvider"
            );

            var testCode = () => validator.ValidateAndThrow(withdraw);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "BUILDER_MUST_BE_APPROVED_IN_LOCK_DEAL_NFT",
                    ErrorMessage = "Provided builder address not approved in the LockDealNFT contract.",
                    CustomState = withdraw
                });
        }

        [Fact]
        internal void WhenAddressIsNotSimpleProvider_ShouldThrowException()
        {
            var withdraw = new ChainAddressPair(97, EthereumAddress.ZeroAddress);

            var validator = new BuilderValidator(
                new MockLockDealNFTContractBuilder()
                    .WithApprovedContract(withdraw.ChainId, withdraw.Address, true)
                    .Build(),
                new MockBuilderContract(isConfigured: false),
                "DealProvider"
            );

            var testCode = () => validator.ValidateAndThrow(withdraw);

            testCode.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new
                {
                    ErrorCode = "BUILDER_MUST_BE_SIMPLE_PROVIDER",
                    ErrorMessage = "Provided builder address is not a simple provider.",
                    CustomState = withdraw
                });
        }

        [Fact]
        internal void WhenBuilderIsValid_ShouldWithoutException()
        {
            var withdraw = new ChainAddressPair(97, EthereumAddress.ZeroAddress);

            var validator = new BuilderValidator(
                new MockLockDealNFTContractBuilder()
                    .WithApprovedContract(withdraw.ChainId, withdraw.Address, true)
                    .Build(),
                new MockBuilderContract(isConfigured: true),
                "DealProvider"
            );

            var testCode = () => validator.ValidateAndThrow(withdraw);

            testCode.Should().NotThrow<ValidationException>();
        }
    }
}