using Newtonsoft.Json;
using Nethereum.Signer;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Tests.Mocks.Services.Handlers.CreateAsset.Models;

internal static class MockCreateAssetRequest
{
    internal static CreateAssetRequest Request => new()
    {
        Signature = Signature,
        Message = Message
    };

    internal static string Signature => new EthereumMessageSigner().EncodeUTF8AndSign(
        message: JsonConvert.SerializeObject(Message, Formatting.None),
        key: MockUsers.Admin.PrivateKey
    );

    internal static string UnauthorizedUserSignature => new EthereumMessageSigner().EncodeUTF8AndSign(
        message: JsonConvert.SerializeObject(Message, Formatting.None),
        key: MockUsers.UnauthorizedUser.PrivateKey
    );

    internal static CreateAssetMessage Message => new()
    {
        PoolId = 1,
        ChainId = 1,
        Users = [
            new User {
                UserAddress = "0x0000000000000000000000000000000000000001",
                WeiAmount = "100000"
            }
        ],
        Schedules = [
            new Schedule {
                ProviderAddress = "0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
                StartDate = DateTimeOffset.FromUnixTimeSeconds(1763586000).DateTime,
                Ratio = 1.0m
            }
        ],
        Refund = new Refund {
            PoolId = 1,
            ChainId = 56,
            Ratio = 2.0m,
            FinishTime = DateTimeOffset.FromUnixTimeSeconds(1763544530).DateTime,
            DealProvider = "0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"
        }
    };
}