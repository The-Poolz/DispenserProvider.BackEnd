using System.Numerics;
using Net.Web3.EthereumWallet;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3.Eip712;

[Struct(name: "Builder")]
public class Builder(EthereumAddress simpleProvider, IEnumerable<BigInteger> @params)
{
    [Parameter(type: "address", name: "simpleProvider", order: 1)]
    public string SimpleProvider { get; } = simpleProvider;

    [Parameter(type: "uint256[]", name: "params", order: 2)]
    public List<BigInteger> Params { get; } = @params.ToList();
}