using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using Net.Web3.EthereumWallet.Json.Converters;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class ChainAddressPair(long chainId, EthereumAddress address)
{
    public long ChainId { get; } = chainId;

    [JsonConverter(typeof(EthereumAddressConverter))]
    public EthereumAddress Address { get; } = address;
}