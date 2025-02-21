using System.Net;

namespace DispenserProvider.Services.Handlers.DeleteAsset.Models;

public class DeleteAssetResponse(HttpStatusCode statusCode)
{
    public DeleteAssetResponse() : this(HttpStatusCode.OK) { }

    public HttpStatusCode StatusCode { get; } = statusCode;
}