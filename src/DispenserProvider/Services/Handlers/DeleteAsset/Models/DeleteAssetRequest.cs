using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Handlers.DeleteAsset.Models;

public class DeleteAssetRequest : ValidatedAdminRequest, IHandlerRequest
{
    [JsonRequired]
    public DeleteAssetMessage Message { get; set; } = null!;

    public override IEnumerable<EthereumAddress> UsersToValidateOrder => Message.Users;
}