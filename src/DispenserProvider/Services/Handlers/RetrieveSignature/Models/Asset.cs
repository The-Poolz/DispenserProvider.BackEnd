using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Handlers.ReadAsset.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class Asset(DispenserDTO dispenser, RetrieveSignatureRequest request)
{
    public string Signature { get; } = dispenser.LastUserSignature!.Signature;

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime ValidUntil { get; } = dispenser.LastUserSignature!.ValidUntil;

    public bool IsRefund { get; } = IsRefundFunc(dispenser, request);

    public Builder[] Builders { get; } = IsRefundFunc(dispenser, request)
        ? dispenser.RefundDetail!.Builders.Select(x => new Builder(x)).ToArray()
        : dispenser.WithdrawalDetail!.Builders.Select(x => new Builder(x)).ToArray();

    private static bool IsRefundFunc(DispenserDTO dispenser, RetrieveSignatureRequest request) => dispenser.RefundDetail != null && 
        dispenser.RefundDetail.ChainId == request.ChainId &&
        dispenser.RefundDetail.PoolId == request.PoolId;
}