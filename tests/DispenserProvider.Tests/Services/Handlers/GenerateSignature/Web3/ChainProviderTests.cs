using Xunit;
using FluentAssertions;
using FluentValidation;
using DispenserProvider.Services.Web3;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Tests.Mocks.DataBase;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Web3;

public class ChainProviderTests
{
    public class Web3
    {
        [Fact]
        internal void WhenChainIdIsSupported_ShouldReturnsWeb3()
        {
            var context = MockCovalentContext.Create();

            var chainProvider = new ChainProvider(context);

            var response = chainProvider.Web3(MockCovalentContext.Chain.ChainId);

            response.Should().NotBeNull();
        }

        [Fact]
        internal void WhenChainIdNotSupported_ShouldThrowException()
        {
            var chainId = 123;
            var context = MockCovalentContext.Create();

            var chainProvider = new ChainProvider(context);

            var testCode = () => chainProvider.Web3(chainId);

            testCode.Should().Throw<ValidationException>()
                .WithMessage(ErrorCode.CHAIN_NOT_SUPPORTED.ToErrorMessage());
        }
    }

    public class ContractAddress
    {
        [Fact]
        internal void WhenChainIdIsSupported_ShouldReturnsContractAddress()
        {
            var context = MockCovalentContext.Create();

            var chainProvider = new ChainProvider(context);

            var response = chainProvider.DispenserProviderContract(MockCovalentContext.Chain.ChainId);

            response.Should().NotBeNull();
        }

        [Fact]
        internal void WhenChainIdNotSupported_ShouldThrowException()
        {
            var chainId = 123;
            var context = MockCovalentContext.Create();

            var chainProvider = new ChainProvider(context);

            var testCode = () => chainProvider.DispenserProviderContract(chainId);

            testCode.Should().Throw<ValidationException>()
                .WithMessage(ErrorCode.DISPENSER_PROVIDER_NOT_SUPPORTED.ToErrorMessage());
        }
    }
}