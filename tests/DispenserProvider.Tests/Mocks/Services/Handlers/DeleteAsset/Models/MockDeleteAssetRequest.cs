using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;

namespace DispenserProvider.Tests.Mocks.Services.Handlers.DeleteAsset.Models;

internal class MockDeleteAssetRequest : MockAssetRequest
{
    internal static DeleteAssetRequest Request => new()
    {
        Signature = Signature,
        Message = Message
    };

    internal static string Signature => GenerateSignature(Message, MockUsers.Admin.PrivateKey);
    internal static string UnauthorizedUserSignature => GenerateSignature(Message, MockUsers.UnauthorizedUser.PrivateKey);
    internal static string SignatureForInvalidMessage => GenerateSignature(InvalidMessage, MockUsers.Admin.PrivateKey);

    internal static DeleteAssetMessage Message => new()
    {
        PoolId = 1,
        ChainId = 1,
        Users = [
            "0x0000000000000000000000000000000000000001"
        ]
    };
    internal static DeleteAssetMessage InvalidMessage => new()
    {
        PoolId = 1,
        ChainId = 1,
        Users = [
            "0x0000000000000000000000000000000000000001",
            "0x0000000000000000000000000000000000000101",
            "0x0000000000000000000000000000000000000102"
        ]
    };
}