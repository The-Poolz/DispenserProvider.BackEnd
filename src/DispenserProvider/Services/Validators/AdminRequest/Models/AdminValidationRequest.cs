using Newtonsoft.Json;
using Nethereum.Signer;
using Net.Web3.EthereumWallet;

namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class AdminValidationRequest(string nameOfRole, ValidatedAdminRequest adminRequest)
{
    public string NameOfRole { get; } = nameOfRole;
    public string RecoveredAddress => new EthereumMessageSigner()
        .EncodeUTF8AndEcRecover(
            message: JsonConvert.SerializeObject(adminRequest.OriginalMessage, Formatting.None),
            signature: adminRequest.Signature
        );

    public IEnumerable<EthereumAddress> UsersToValidateOrder => adminRequest.UsersToValidateOrder;
}