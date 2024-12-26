using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using Newtonsoft.Json.Converters;
using TokenSchedule.FluentValidation.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class Schedule : IValidatedScheduleItem
{
    [JsonRequired]
    public EthereumAddress ProviderAddress { get; set; } = null!;

    [JsonRequired]
    public decimal Ratio { get; set; }

    [JsonRequired]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    [JsonProperty("StartTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime StartDate { get; set; }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    [JsonProperty("FinishTime", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? FinishDate { get; set; }
}