using MediatR;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class CreateAssetRequest : ValidatedAdminRequest<CreateAssetMessage>, IRequest<CreateAssetResponse>;