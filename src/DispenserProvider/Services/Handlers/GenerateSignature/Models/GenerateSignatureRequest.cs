using MediatR;
using Newtonsoft.Json;
using Net.Web3.EthereumWallet;
using DispenserProvider.Services.Database.Models;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Models;

public class GenerateSignatureRequest : IGetDispenserRequest, IRequest<GenerateSignatureResponse>
{
    [JsonRequired]
    public EthereumAddress UserAddress { get; set; } = null!;

    [JsonRequired]
    public long PoolId { get; set; }

    [JsonRequired]
    public long ChainId { get; set; }
}