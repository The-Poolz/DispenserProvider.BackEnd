using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.DeleteAsset.Models;

public class DeleteAssetMessage
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
}