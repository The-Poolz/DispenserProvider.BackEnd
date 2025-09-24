using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Models;

public class GenerateSignatureResponse(DateTime validFrom)
{
    public GenerateSignatureResponse() : this(default) { }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime ValidFrom { get; } = validFrom;
}