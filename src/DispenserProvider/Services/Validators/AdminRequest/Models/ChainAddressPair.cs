using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class ChainAddressPair(long chainId, EthereumAddress address)
{
    public long ChainId { get; } = chainId;
    public EthereumAddress Address { get; } = address;
}