using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Models;
using DispenserProvider.Services.Database.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class RetrieveSignatureRequest : IGetDispenserRequest, IHandlerRequest
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }
}