using CovalentDb;
using Nethereum.Web3;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public class ChainProvider(CovalentContext context)
{
    private readonly Dictionary<long, EthereumAddress> contractsAddresses = new()
    {
        { 1, "0x55eB3e27355c09854f7F85371600C360Bd95d42F" },
        { 56, "0x55eB3e27355c09854f7F85371600C360Bd95d42F" },
        { 97, "0x55eB3e27355c09854f7F85371600C360Bd95d42F" }
    };

    public IWeb3 Web3(long chainId)
    {
        var chain = context.Chains.FirstOrDefault(x => x.ChainId == chainId)
            ?? throw new InvalidOperationException($"ChainId={chainId}, not supported.");
        return new Nethereum.Web3.Web3(chain.RpcConnection);
    }

    public EthereumAddress ContractAddress(long chainId)
    {
        return contractsAddresses[chainId];
    }
}