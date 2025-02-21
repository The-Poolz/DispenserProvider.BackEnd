using System.Net;
using DispenserProvider.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models;

public class CreateAssetResponse(HttpStatusCode statusCode) : IHandlerResponse
{
    public CreateAssetResponse() : this(HttpStatusCode.OK) { }
    
    public HttpStatusCode StatusCode { get; } = statusCode;
}