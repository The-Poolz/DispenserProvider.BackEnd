using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Handlers.ReadAsset.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class Asset(DispenserDTO dispenser, TransactionDetailDTO transactionDetail, bool isRefund)
{
    public string Receiver { get; } = dispenser.UserAddress;

    public long PoolId { get; } = transactionDetail.PoolId;

    public long ChainId { get; } = transactionDetail.ChainId;

    public string Signature { get; } = dispenser.LastUserSignature!.Signature;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime ValidUntil { get; } = dispenser.LastUserSignature!.ValidUntil;

    public bool IsRefund { get; } = isRefund;

    public Builder[] Builders { get; } = transactionDetail.Builders.Select(x => new Builder(x)).ToArray();
}