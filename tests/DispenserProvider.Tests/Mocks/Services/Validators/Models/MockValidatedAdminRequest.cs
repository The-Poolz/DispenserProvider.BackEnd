using Nethereum.Signer;
using Nethereum.ABI.EIP712;
using Nethereum.Signer.EIP712;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.MessageTemplate.Models.Eip712;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Mocks.Services.Validators.Models;

internal static class MockValidatedAdminRequest
{
    internal static ValidatedAdminRequest<MockValidatedMessage> Create() => Create(new MockValidatedMessage(), MockUsers.Admin.PrivateKey);

    internal static ValidatedAdminRequest<TMessage> Create<TMessage>(TMessage plainMessage, EthECKey privateKey)
        where TMessage : IValidatedMessage
    {
        var signature = new Eip712TypedDataSigner().SignTypedDataV4<EIP712Domain>(plainMessage.Eip712Message.TypedData.ToJson(), privateKey);
        return new ValidatedAdminRequest<TMessage>
        {
            Message = plainMessage,
            Signature = signature
        };
    }
}