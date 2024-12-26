using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class Asset(DispenserDTO dispenser)
{
    public string UserAddress { get; } = dispenser.UserAddress;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? RefundFinishTime { get; } = dispenser.RefundFinishTime;

    public TransactionDetail WithdrawalDetail { get; } = new(dispenser.WithdrawalDetail);

    public TransactionDetail? RefundDetail { get; } = dispenser.RefundDetail == null ? null : new TransactionDetail(dispenser.RefundDetail);
}