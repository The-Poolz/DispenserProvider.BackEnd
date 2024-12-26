using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Handlers.DeleteAsset.Models;

public class DeleteAssetMessage : IPlainMessage
{
    [JsonRequired]
    public long ChainId { get; set; }

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonRequired]
    public EthereumAddress[] Users { get; set; } = null!;

    [JsonIgnore]
    internal IDictionary<EthereumAddress, string> ToDelete => Users.ToDictionary(
        key => key,
        value => DispenserDTO.GenerateId(value.Address, ChainId, PoolId)
    );

    [JsonIgnore]
    public IEnumerable<EthereumAddress> UsersToValidateOrder => Users;
}