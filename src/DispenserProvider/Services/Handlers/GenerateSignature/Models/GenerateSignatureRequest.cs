using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Models;

public class GenerateSignatureRequest : IHandlerRequest
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }
}