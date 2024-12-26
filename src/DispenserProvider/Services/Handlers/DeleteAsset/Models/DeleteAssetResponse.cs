using System.Net;
using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.DeleteAsset.Models;

public class DeleteAssetResponse(HttpStatusCode statusCode) : IHandlerResponse
{
    public DeleteAssetResponse() : this(HttpStatusCode.OK) { }

    public HttpStatusCode StatusCode { get; } = statusCode;
}