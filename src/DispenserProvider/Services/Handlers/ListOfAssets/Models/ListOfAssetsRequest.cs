using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class ListOfAssetsRequest : IHandlerRequest
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;
}