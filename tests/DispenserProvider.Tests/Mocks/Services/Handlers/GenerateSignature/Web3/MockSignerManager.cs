using Nethereum.Signer;
using DispenserProvider.Services.Web3;

namespace DispenserProvider.Tests.Mocks.Services.Handlers.GenerateSignature.Web3
{
    internal class MockSignerManager(EthECKey ethEcKey) : ISignerManager
    {
        public MockSignerManager(string ethEcKey)
            : this(new EthECKey(ethEcKey))
        { }

        public EthECKey GetSigner() => ethEcKey;
    }
}
