using Moq;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Web3;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Mocks.Services.Web3;

internal class MockLockDealNFTContract
{
    public static ILockDealNFTContract Create(CreateAssetMessage message, EthereumAddress? ownerOfWithdraw = null, EthereumAddress? ownerOfRefund = null)
    {
        return Create(
            withdraw: new ChainPoolPair(
                message.ChainId,
                message.PoolId
            ),
            refund: message.Refund == null ? null : new ChainPoolPair(
                message.Refund.ChainId,
                message.Refund.PoolId
            ),
            ownerOfWithdraw,
            ownerOfRefund
        );
    }

    public static ILockDealNFTContract Create(PoolOwnershipValidatorRequest request, EthereumAddress? ownerOfWithdraw = null, EthereumAddress? ownerOfRefund = null)
    {
        return Create(
        withdraw: new ChainPoolPair(
            request.Withdraw.ChainId,
            request.Withdraw.PoolId
        ),
            refund: request.Refund == null ? null : new ChainPoolPair(
                request.Refund.ChainId,
                request.Refund.PoolId
            ),
            ownerOfWithdraw,
            ownerOfRefund
        );
    }

    public static ILockDealNFTContract Create(ChainPoolPair withdraw, ChainPoolPair? refund = null, EthereumAddress? ownerOfWithdraw = null, EthereumAddress? ownerOfRefund = null)
    {
        var lockDealNFT = new Mock<ILockDealNFTContract>();

        lockDealNFT.Setup(x => x.OwnerOf(withdraw.ChainId, withdraw.PoolId))
            .Returns(ownerOfWithdraw ?? EthereumAddress.ZeroAddress);

        if (refund != null)
        {
            lockDealNFT.Setup(x => x.OwnerOf(refund.ChainId, refund.PoolId))
                .Returns(ownerOfRefund ?? EthereumAddress.ZeroAddress);
        }

        return lockDealNFT.Object;
    }
}