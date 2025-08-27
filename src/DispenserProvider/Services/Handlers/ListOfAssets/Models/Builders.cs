using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class Builder(BuilderDTO builder)
{
    public string ProviderAddress { get; } = builder.ProviderAddress;

    public string WeiAmount { get; } = builder.WeiAmount;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? StartTime { get; } = builder.StartTime.HasValue? DateTime.SpecifyKind(builder.StartTime.Value, DateTimeKind.Utc) : null;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? FinishTime { get; } = builder.FinishTime.HasValue? DateTime.SpecifyKind(builder.FinishTime.Value, DateTimeKind.Utc) : null;
}