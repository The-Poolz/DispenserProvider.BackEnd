using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.ReadAsset.Models;

public class Dispenser(DispenserDTO dispenser, bool isRefund)
{
    public bool IsTaken { get; } = dispenser.TakenTrack != null;

    public string UserAddress { get; } = dispenser.UserAddress;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? RefundFinishTime { get; } = dispenser.RefundFinishTime;

    public IEnumerable<Builder> Builders { get; } = isRefund ?
        dispenser.RefundDetail!.Builders.Select(x => new Builder(x)).ToArray() :
        dispenser.WithdrawalDetail.Builders.Select(x => new Builder(x)).ToArray();

    [JsonIgnore]
    internal DispenserDTO DTO { get; } = dispenser;
}