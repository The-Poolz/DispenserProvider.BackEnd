using Nethereum.Signer;
using SecretsManager;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public interface ISignerManager
{
    public EthECKey GetSigner(SecretManager secretManager);
}