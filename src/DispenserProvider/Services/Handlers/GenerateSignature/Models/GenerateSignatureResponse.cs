using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DispenserProvider.Services.Handlers.GenerateSignature.Models;

public class GenerateSignatureResponse(DateTimeOffset validFrom)
{
    public GenerateSignatureResponse() : this(default) { }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTimeOffset ValidFrom { get; } = validFrom;
}