using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace DispenserProvider.Services.Web3.MultiCall.Models;

[Function("multicall", "bytes[]")]
public class MultiCallFunction : FunctionMessage
{
    [Parameter("tuple[]", "calls", order: 1)]
    public List<MultiCall> Calls { get; set; } = [];
}