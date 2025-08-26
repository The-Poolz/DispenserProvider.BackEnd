using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class Builder(BuilderDTO builder)
{
    public string ProviderAddress { get; } = builder.ProviderAddress;

    public string WeiAmount { get; } = builder.WeiAmount;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? StartTime { get; } = builder.StartTime?.ToUniversalTime();

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? FinishTime { get; } = builder.FinishTime?.ToUniversalTime();
}