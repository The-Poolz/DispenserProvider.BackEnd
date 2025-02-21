using MediatR;
using DispenserProvider.Services.Validators.AdminRequest.Models;

namespace DispenserProvider.Services.Handlers.DeleteAsset.Models;

public class DeleteAssetRequest : ValidatedAdminRequest<DeleteAssetMessage>, IRequest<DeleteAssetResponse>;