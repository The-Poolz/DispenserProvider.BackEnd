using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Tests.Mocks.Services.Handlers.CreateAsset.Models;

internal class MockCreateAssetRequest : MockAssetRequest
{
    internal static CreateAssetRequest Request => new()
    {
        Signature = Signature,
        Message = Message
    };

    internal static string Signature => GenerateSignature(Message, MockUsers.Admin.PrivateKey);
    internal static string UnauthorizedUserSignature => GenerateSignature(Message, MockUsers.UnauthorizedUser.PrivateKey);

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
                StartDate = DateTimeOffset.FromUnixTimeSeconds(1763586000).UtcDateTime,
                FinishDate = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime,
                Ratio = "1000000000000000000"
            }
        ],
        Refund = new Refund {
            PoolId = 1,
            ChainId = 56,
            Ratio = "2000000000000000000",
            FinishTime = DateTimeOffset.FromUnixTimeSeconds(1763544530).UtcDateTime,
            DealProvider = "0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"
        }
    };
}