using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using Newtonsoft.Json.Converters;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class Refund
{
    [JsonRequired]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTimeOffset FinishTime { get; set; }

    [JsonRequired]
    public string WeiRatio { get; set; } = null!;

    [JsonRequired]
    public EthereumAddress DealProvider { get; set; } = null!;

    [JsonRequired]
    public long ChainId { get; set; }

    [JsonRequired]
    public long PoolId { get; set; }
}