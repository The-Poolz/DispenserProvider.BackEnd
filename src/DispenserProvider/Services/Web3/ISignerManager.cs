using Nethereum.Signer;

namespace DispenserProvider.Services.Web3;

public interface ISignerManager
{
    public EthECKey GetSigner();
}