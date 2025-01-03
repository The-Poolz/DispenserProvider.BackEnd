using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using Newtonsoft.Json.Converters;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Handlers.ReadAsset.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class Asset(DispenserDTO dispenser, bool isRefund)
{
    public EthereumAddress Receiver { get; } = dispenser.UserAddress;

    public long PoolId { get; } = isRefund ? dispenser.RefundDetail!.PoolId : dispenser.WithdrawalDetail.PoolId;

    public long ChainId { get; } = isRefund ? dispenser.RefundDetail!.ChainId : dispenser.WithdrawalDetail.ChainId;

    public string Signature { get; } = dispenser.LastUserSignature!.Signature;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime ValidUntil { get; } = dispenser.LastUserSignature!.ValidUntil;

    public bool IsRefund { get; } = isRefund;

    public Builder[] Builders { get; } = isRefund
        ? dispenser.RefundDetail!.Builders.Select(x => new Builder(x)).ToArray()
        : dispenser.WithdrawalDetail!.Builders.Select(x => new Builder(x)).ToArray();
}