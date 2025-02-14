using System.Numerics;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Web3;

public interface ILockDealNFTContract
{
    public EthereumAddress OwnerOf(long chainId, BigInteger tokenId);
}