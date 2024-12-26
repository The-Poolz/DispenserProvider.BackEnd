using Newtonsoft.Json;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class User
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;

    [JsonRequired]
    public string WeiAmount { get; set; } = null!;
}