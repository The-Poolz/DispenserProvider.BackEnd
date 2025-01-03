using SecretsManager;
using Nethereum.Signer;
using EnvironmentManager.Static;
using EnvironmentManager.Extensions;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public class SignerManager(bool isProduction = true) : ISignerManager
{
    public EthECKey GetSigner(SecretManager secretManager)
    {
        var privateKey = !isProduction
            ? EnvManager.GetRequired<string>("PRIVATE_KEY")
            : secretManager.GetSecretValue(
                secretId: Env.SECRET_ID_OF_SIGN_ACCOUNT.GetRequired<string>(),
                secretKey: Env.SECRET_KEY_OF_SIGN_ACCOUNT.GetRequired<string>()
            );

        return new EthECKey(privateKey);
    }
}