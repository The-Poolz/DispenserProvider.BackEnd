using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class CreateAssetMessage : IPlainMessage
{
    [JsonRequired]
    public Schedule[] Schedules { get; set; } = [];

    [JsonRequired]
    public User[] Users { get; set; } = [];

    public Refund? Refund { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonIgnore]
    public IEnumerable<EthereumAddress> UsersToValidateOrder => Users.Select(x => x.UserAddress);
}