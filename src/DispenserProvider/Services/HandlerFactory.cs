using DispenserProvider.Models;
using DispenserProvider.Services.Handlers;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Handlers.ReadAsset.Models;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

namespace DispenserProvider.Services;

public class HandlerFactory(IServiceProvider serviceProvider) : IHandlerFactory
{
    public IHandlerResponse Handle(LambdaRequest request)
    {
        return request switch
        {
            { CreateAssetRequest: not null } => serviceProvider.GetRequiredService<IRequestHandler<CreateAssetRequest, CreateAssetResponse>>().Handle(request.CreateAssetRequest),
            { DeleteAssetRequest: not null } => serviceProvider.GetRequiredService<IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>>().Handle(request.DeleteAssetRequest),
            { ReadAssetRequest: not null } => serviceProvider.GetRequiredService<IRequestHandler<ReadAssetRequest, ReadAssetResponse>>().Handle(request.ReadAssetRequest),
            { ListOfAssetsRequest: not null } => serviceProvider.GetRequiredService<IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>>().Handle(request.ListOfAssetsRequest),
            { GenerateSignatureRequest: not null } => serviceProvider.GetRequiredService<IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>>().Handle(request.GenerateSignatureRequest),
            { RetrieveSignatureRequest: not null } => serviceProvider.GetRequiredService<IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>>().Handle(request.RetrieveSignatureRequest),
            _ => throw new NotImplementedException("No one implemented request found.")
        };
    }
}