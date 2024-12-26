using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class Dispenser(DispenserDTO dispenser, IEnumerable<BuilderDTO> builders)
{
    public string UserAddress { get; } = dispenser.UserAddress;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? RefundFinishTime { get; } = dispenser.RefundFinishTime;

    public Builder[] Builders { get; } = builders.Select(x => new Builder(x)).ToArray();
}