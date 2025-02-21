using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Models;
using DispenserProvider.Extensions.Pagination;

namespace DispenserProvider.Services.Handlers.ListOfAssets.Models;

public class ListOfAssetsRequest : IPaginated, IHandlerRequest<ListOfAssetsResponse>
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;

    [JsonRequired]
    public int Limit { get; set; } = 1;

    [JsonRequired]
    public int Page { get; set; } = 1;
}