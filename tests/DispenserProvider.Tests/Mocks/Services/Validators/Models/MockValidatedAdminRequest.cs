using Newtonsoft.Json;
using Nethereum.Signer;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Tests.Mocks.Services.Validators.Models;

internal static class MockValidatedAdminRequest
{
    internal static ValidatedAdminRequest<MockPlainMessage> Create() => Create(new MockPlainMessage(), MockUsers.Admin.PrivateKey);

    internal static ValidatedAdminRequest<TMessage> Create<TMessage>(TMessage plainMessage, EthECKey privateKey)
        where TMessage : IPlainMessage
    {
        var message = JsonConvert.SerializeObject(plainMessage, Formatting.None);
        var signature = new EthereumMessageSigner().EncodeUTF8AndSign(message, privateKey);
        return new ValidatedAdminRequest<TMessage>
        {
            Message = plainMessage,
            Signature = signature
        };
    }
}