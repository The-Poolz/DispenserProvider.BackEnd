using Nethereum.ABI;
using SecretsManager;
using System.Numerics;
using Nethereum.Signer;
using EnvironmentManager.Static;
using EnvironmentManager.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Helpers;

public class SignatureGenerator(SecretManager secretManager) : ISignatureGenerator
{
    public string GenerateSignature(TransactionDetailDTO transactionDetail, DateTime validUntil)
    {
        var packedData = new ABIEncode().GetSha3ABIEncodedPacked(
            abiValues: BuildAbiValues(transactionDetail, validUntil)
        );

        var signature = new EthereumMessageSigner().Sign(packedData, GetSigner());
        return signature;
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

    private EthECKey GetSigner()
    {
        var isProduction = EnvManager.Get<bool?>("IS_PROD");
        var privateKey = isProduction.HasValue && !isProduction.Value
            ? EnvManager.GetRequired<string>("PRIVATE_KEY")
            : secretManager.GetSecretValue(
                secretId: Env.SECRET_ID_OF_SIGN_ACCOUNT.GetRequired<string>(),
                secretKey: Env.SECRET_KEY_OF_SIGN_ACCOUNT.GetRequired<string>()
            );

        return new EthECKey(privateKey);
    }
}