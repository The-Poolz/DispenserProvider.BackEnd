using System.Numerics;
using Net.Web3.EthereumWallet;
using DispenserProvider.Extensions;
using DispenserProvider.DataBase.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3.Eip712;

[Struct(name: "MessageStruct")]
public class SignatureStruct(long poolId, EthereumAddress receiver, DateTimeOffset validUntil, IEnumerable<Builder> builders)
{
    public SignatureStruct(long poolId, EthereumAddress receiver, long validUntil, IEnumerable<Builder> builders)
        : this(poolId, receiver, DateTimeOffset.FromUnixTimeSeconds(validUntil), builders)
    { }

    public SignatureStruct(TransactionDetailDTO transactionDetail, DateTimeOffset validUntil)
        : this(
            transactionDetail.PoolId,
            transactionDetail.UserAddress,
            validUntil,
            transactionDetail.Builders.Select(x =>
            {
                var @params = new List<BigInteger> { BigInteger.Parse(x.WeiAmount) };
                if (x.StartTime.HasValue) @params.Add(x.StartTime.Value.ToUnixTimeSeconds());
                if (x.FinishTime.HasValue) @params.Add(x.FinishTime.Value.ToUnixTimeSeconds());
                return new Builder(x.ProviderAddress, @params);
            })
        )
    { }

    [Parameter(type: "uint256", name: "poolId", order: 1)]
    public BigInteger PoolId { get; } = poolId;

    [Parameter(type: "address", name: "receiver", order: 2)]
    public string Receiver { get; } = receiver;

    [Parameter(type: "uint256", name: "validUntil", order: 3)]
    public BigInteger ValidUntil { get; } = validUntil.SpecifyUtcKind().RoundDateTime().ToUnixTimestamp();

    [Parameter(type: "tuple[]", name: "data", order: 4, structTypeName: "Builder[]")]
    public List<Builder> Builders { get; } = builders.ToList();

    private static BigInteger ToUnixBigInteger(DateTime date)
    {
        return new BigInteger(date.SpecifyUtcKind().RoundDateTime().ToUnixTimestamp());
    }
}