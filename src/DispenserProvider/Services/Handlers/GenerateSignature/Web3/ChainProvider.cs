using CovalentDb;
using Nethereum.Web3;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public class ChainProvider(CovalentContext context) : IChainProvider
{
    private readonly Dictionary<long, EthereumAddress> _contractsAddresses = new()
    {
        { 1, "0x8080092b3BB79564EF59193f5417421A41E52b99" },
        { 56, "0x8080092b3BB79564EF59193f5417421A41E52b99" },
        { 97, "0x8080092b3BB79564EF59193f5417421A41E52b99" }
    };

    public IWeb3 Web3(long chainId)
    {
        var chain = context.Chains.FirstOrDefault(x => x.ChainId == chainId)
            ?? throw new InvalidOperationException($"ChainId={chainId}, not supported.");
        return new Nethereum.Web3.Web3(chain.RpcConnection);
    }

    public EthereumAddress ContractAddress(long chainId)
    {
        return _contractsAddresses[chainId];
    }
}