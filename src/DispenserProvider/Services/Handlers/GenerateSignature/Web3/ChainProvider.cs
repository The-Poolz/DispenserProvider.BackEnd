using CovalentDb;
using Nethereum.Web3;
using Net.Web3.EthereumWallet;
using DispenserProvider.Extensions;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public class ChainProvider(CovalentContext context) : IChainProvider
{
    private readonly Dictionary<long, EthereumAddress> _contractsAddresses = new()
    {
        { 1, "0x55eB3e27355c09854f7F85371600C360Bd95d42F" },
        { 56, "0x55eB3e27355c09854f7F85371600C360Bd95d42F" },
        { 97, "0x55eB3e27355c09854f7F85371600C360Bd95d42F" }
    };

    public IWeb3 Web3(long chainId)
    {
        var chain = context.Chains.FirstOrDefault(x => x.ChainId == chainId)
            ?? throw ErrorCode.CHAIN_NOT_SUPPORTED.ToException(new
            {
                ChainId = chainId
            });
        return new Nethereum.Web3.Web3(chain.RpcConnection);
    }

    public EthereumAddress ContractAddress(long chainId)
    {
        return _contractsAddresses[chainId];
    }
}