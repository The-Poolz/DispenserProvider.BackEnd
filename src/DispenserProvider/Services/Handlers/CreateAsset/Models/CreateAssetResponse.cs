using System.Net;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class CreateAssetResponse(HttpStatusCode statusCode)
{
    public CreateAssetResponse() : this(HttpStatusCode.OK) { }
    
    public HttpStatusCode StatusCode { get; } = statusCode;
}