using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class CreateAssetRequest : ValidatedAdminRequest, IHandlerRequest
{
    [JsonRequired]
    public CreateAssetMessage Message { get; set; } = default!;

    [JsonIgnore]
    public override IEnumerable<EthereumAddress> UsersToValidateOrder => Message.Users.Select(x => x.UserAddress);
}