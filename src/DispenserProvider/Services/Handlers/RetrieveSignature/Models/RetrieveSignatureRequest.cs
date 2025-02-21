using MediatR;
using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Database.Models;
using DispenserProvider.Services.Validators.Signature.Models;

namespace DispenserProvider.Services.Handlers.RetrieveSignature.Models;

public class RetrieveSignatureRequest : IGetDispenserRequest, IRequest<RetrieveSignatureResponse>
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }

    [JsonIgnore]
    public RetrieveSignatureValidatorRequest ValidatorRequest => _validatorRequest.Value;
    private Lazy<RetrieveSignatureValidatorRequest> _validatorRequest = null!;
    public void InitializeValidatorRequest(IDispenserManager dispenserManager)
    {
        _validatorRequest = new Lazy<RetrieveSignatureValidatorRequest>(() =>
        {
            var dispenser = dispenserManager.GetDispenser(this);
            var isRefund = dispenser.RefundDetail != null && dispenser.RefundDetail.ChainId == ChainId && dispenser.RefundDetail.PoolId == PoolId;
            return new RetrieveSignatureValidatorRequest(dispenser, isRefund);
        });
    }
}