using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Database.Models;

public interface IGetDispenserRequest
{
    public EthereumAddress UserAddress { get; }
    public long PoolId { get; }
    public long ChainId { get; }
}