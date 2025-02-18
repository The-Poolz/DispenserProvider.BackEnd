using System.Numerics;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Web3.Contracts;

public interface ILockDealNFTContract
{
    public EthereumAddress OwnerOf(long chainId, BigInteger tokenId);
    public bool ApprovedContract(long chainId, EthereumAddress address);
}