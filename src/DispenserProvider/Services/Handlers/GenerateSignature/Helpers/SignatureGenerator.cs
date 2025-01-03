using Nethereum.ABI;
using System.Numerics;
using Nethereum.Signer;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Helpers;

public class SignatureGenerator(ISignerManager signerManager) : ISignatureGenerator
{
    public string GenerateSignature(TransactionDetailDTO transactionDetail, DateTime validUntil)
    {
        var packedData = new ABIEncode().GetSha3ABIEncodedPacked(
            abiValues: BuildAbiValues(transactionDetail, validUntil)
        );

        return new EthereumMessageSigner().Sign(
            message: packedData, 
            key: signerManager.GetSigner()
        );
    }

    private static ABIValue[] BuildAbiValues(TransactionDetailDTO transactionDetail, DateTime validUntil)
    {
        var data = new List<ABIValue>
        {
            new("uint256", new BigInteger(transactionDetail.PoolId)),
            new("uint256", ToUnixBigInteger(validUntil)),
            new("address", (transactionDetail.RefundDispenser ?? transactionDetail.WithdrawalDispenser)!.UserAddress)
        };

        var builderValues = transactionDetail.Builders.SelectMany(builder =>
        {
            var values = new List<object> { BigInteger.Parse(builder.WeiAmount) };
            if (builder.StartTime.HasValue) values.Add(ToUnixBigInteger(builder.StartTime.Value));
            if (builder.FinishTime.HasValue) values.Add(ToUnixBigInteger(builder.FinishTime.Value));

            return new[]
            {
                new ABIValue("address", builder.ProviderAddress),
                new ABIValue("uint256[]", values.ToArray())
            };
        });

        data.AddRange(builderValues);

        return data.ToArray();
    }

    private static BigInteger ToUnixBigInteger(DateTime date) => new(new DateTimeOffset(date).ToUnixTimeSeconds());
}