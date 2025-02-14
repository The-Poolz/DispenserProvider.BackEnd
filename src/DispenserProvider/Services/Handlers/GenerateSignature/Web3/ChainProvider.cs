using CovalentDb;
using Nethereum.Web3;
using Net.Web3.EthereumWallet;
using DispenserProvider.Extensions;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public class ChainProvider(CovalentContext context) : IChainProvider
{
    private readonly Dictionary<long, EthereumAddress> _contractsAddresses = new()
    {
        { 1, "0xa9c68640C1AA52E91A75F4c5e2786F68049541Ad" },
        { 56, "0xa9c68640C1AA52E91A75F4c5e2786F68049541Ad" },
        { 97, "0xa9c68640C1AA52E91A75F4c5e2786F68049541Ad" }
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