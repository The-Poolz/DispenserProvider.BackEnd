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
    public GenerateSignatureRequest? GenerateSignatureRequest { get; set; }
    public RetrieveSignatureRequest? RetrieveSignatureRequest { get; set; }

    public IHandlerRequest<IHandlerResponse> Request => this switch
    {
        { CreateAssetRequest: not null } => CreateAssetRequest,
        { DeleteAssetRequest: not null } => DeleteAssetRequest,
        { ReadAssetRequest: not null } => ReadAssetRequest,
        { ListOfAssetsRequest: not null } => ListOfAssetsRequest,
        { GenerateSignatureRequest: not null } => GenerateSignatureRequest,
        { RetrieveSignatureRequest: not null } => RetrieveSignatureRequest,
        _ => throw ErrorCode.INVALID_HANDLER_REQUEST.ToException()
    };
}