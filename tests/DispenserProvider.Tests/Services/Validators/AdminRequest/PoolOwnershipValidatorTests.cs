using Moq;
using Xunit;
using FluentAssertions;
using FluentValidation;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Web3;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Tests.Mocks.Services.Web3;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Tests.Mocks.DataBase;

namespace DispenserProvider.Tests.Services.Validators.AdminRequest;

public class PoolOwnershipValidatorTests
{
    public class ValidateAndThrow
    {
        [Fact]
        internal void WhenAssetAlreadyWithdrawn_ShouldThrowException()
        {
            var withdraw = new ChainPoolPair(1, 1);
            var refund = new ChainPoolPair(56, 1);
            var request = new PoolOwnershipValidatorRequest(withdraw, refund);

            var signerManager = new MockSignerManager(MockUsers.Admin.PrivateKey);
            var lockDealNFT = new Mock<ILockDealNFTContract>();
            lockDealNFT.Setup(x => x.OwnerOf(withdraw.ChainId, withdraw.PoolId))
                .Returns(new EthereumAddress("0x" + new string('1', 40)));
            lockDealNFT.Setup(x => x.OwnerOf(refund.ChainId, refund.PoolId))
                .Returns(new EthereumAddress("0x" + new string('2', 40)));

            var validator = new PoolOwnershipValidator(signerManager, lockDealNFT.Object);

            var testCode = () => validator.ValidateAndThrow(request);

            testCode.Should().Throw<ValidationException>()
                .WithMessage($"*{ErrorCode.INVALID_TOKEN_OWNER.ToErrorMessage()}*");
        }
    }
}