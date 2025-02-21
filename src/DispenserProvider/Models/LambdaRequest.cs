using MediatR;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Handlers.ReadAsset.Models;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Models;

public class LambdaRequest
{
    public ReadAssetRequest? ReadAssetRequest { get; set; }
    public CreateAssetRequest? CreateAssetRequest { get; set; }
    public DeleteAssetRequest? DeleteAssetRequest { get; set; }

    public ListOfAssetsRequest? ListOfAssetsRequest { get; set; }
    public SignatureRequest? GenerateSignatureRequest { get; set; }
    public SignatureRequest? RetrieveSignatureRequest { get; set; }

    public IRequest<IHandlerResponse> GetRequest(IServiceProvider serviceProvider) => this switch
    {
        { CreateAssetRequest: not null } => CreateAssetRequest,
        { DeleteAssetRequest: not null } => DeleteAssetRequest,
        { ReadAssetRequest: not null } => ReadAssetRequest,
        { ListOfAssetsRequest: not null } => ListOfAssetsRequest,
        { GenerateSignatureRequest: not null } => new GenerateSignatureRequest(GenerateSignatureRequest, serviceProvider),
        { RetrieveSignatureRequest: not null } => new RetrieveSignatureRequest(RetrieveSignatureRequest, serviceProvider),
        _ => throw ErrorCode.INVALID_HANDLER_REQUEST.ToException()
    };
}