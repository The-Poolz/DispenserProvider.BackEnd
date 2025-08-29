using Nethereum.ABI.FunctionEncoding.Attributes;

namespace DispenserProvider.Services.Web3.MultiCall.Models;

public class MultiCall
{
    [Parameter("address", "to", order: 1)]
    public string To { get; set; } = "";

    [Parameter("bytes", "data", order: 2)]
    public byte[] Data { get; set; } = [];
}