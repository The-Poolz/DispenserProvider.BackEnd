using Newtonsoft.Json;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public interface IPlainMessage
{
    [JsonIgnore]
    public IEnumerable<EthereumAddress> UsersToValidateOrder { get; }
}