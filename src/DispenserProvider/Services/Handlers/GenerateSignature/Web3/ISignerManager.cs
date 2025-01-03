using Nethereum.Signer;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Web3;

public interface ISignerManager
{
    public EthECKey GetSigner();
}