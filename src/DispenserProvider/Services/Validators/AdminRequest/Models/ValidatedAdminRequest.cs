using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public abstract class ValidatedAdminRequest
{
    [JsonRequired]
    public string Signature { get; set; } = null!;

    [JsonRequired]
    public JObject OriginalMessage { get; set; } = default!;

    [JsonIgnore]
    public abstract IEnumerable<EthereumAddress> UsersToValidateOrder { get; }
}