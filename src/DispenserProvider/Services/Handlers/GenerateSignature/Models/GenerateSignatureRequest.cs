using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Models;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Database.Models;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Models;

public class GenerateSignatureRequest : IGetDispenserRequest, IHandlerRequest<GenerateSignatureResponse>
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }

    [JsonIgnore]
    public GenerateSignatureValidatorRequest ValidatorRequest => _validatorRequest.Value;
    private Lazy<GenerateSignatureValidatorRequest> _validatorRequest = null!;
    public void InitializeValidatorRequest(IDispenserManager dispenserManager)
    {
        _validatorRequest = new Lazy<GenerateSignatureValidatorRequest>(() =>
        {
            var dispenser = dispenserManager.GetDispenser(this);
            var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == ChainId && dispenser.RefundDetail.PoolId == PoolId;
            return new GenerateSignatureValidatorRequest(dispenser, isRefund);
        });
    }
}