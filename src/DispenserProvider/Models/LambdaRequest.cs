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
}