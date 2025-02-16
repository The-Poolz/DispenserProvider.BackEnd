using Nethereum.Signer;
using EnvironmentManager.Static;

namespace DispenserProvider.Services.Web3;

public class EnvSignerManager : ISignerManager
{
    public EthECKey GetSigner()
    {
        return new EthECKey(EnvManager.GetRequired<string>("PRIVATE_KEY"));
    }
}