using DispenserProvider.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class CreateAssetRequest : ValidatedAdminRequest<CreateAssetMessage>, IHandlerRequest;