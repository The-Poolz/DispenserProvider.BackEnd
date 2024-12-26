using Newtonsoft.Json;
using DispenserProvider.Models;
using Newtonsoft.Json.Converters;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Models;

public class GenerateSignatureResponse(DateTime validFrom) : IHandlerResponse
{
    public GenerateSignatureResponse() : this(default) { }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime ValidFrom { get; } = validFrom;
}