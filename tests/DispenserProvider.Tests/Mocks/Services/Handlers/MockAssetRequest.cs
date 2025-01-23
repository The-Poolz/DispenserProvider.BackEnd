using Nethereum.Signer;
using Nethereum.ABI.EIP712;
using Nethereum.Signer.EIP712;
using DispenserProvider.MessageTemplate.Models.Eip712;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Mocks.Services.Handlers;

internal class MockAssetRequest
{
    protected static string GenerateSignature<TMessage>(TMessage message, EthECKey key)
        where TMessage : IValidatedMessage
    {
        return new Eip712TypedDataSigner().SignTypedDataV4<EIP712Domain>(message.Eip712Message.TypedData.ToJson(), key);
    }
}