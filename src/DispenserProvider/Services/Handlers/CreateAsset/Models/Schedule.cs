using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using Newtonsoft.Json.Converters;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class Schedule
{
    [JsonRequired]
    public EthereumAddress ProviderAddress { get; set; } = null!;

    [JsonRequired]
    public string WeiRatio { get; set; } = null!;

    [JsonRequired]
    [JsonProperty("StartTime")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime StartDate { get; set; }

    [JsonRequired]
    [JsonProperty("FinishTime")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime FinishDate { get; set; }
}