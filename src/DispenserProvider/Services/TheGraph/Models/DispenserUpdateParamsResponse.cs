using Newtonsoft.Json;
using Poolz.Finance.CSharp.TheGraph;

namespace DispenserProvider.Services.TheGraph.Models;

[method: JsonConstructor]
public record DispenserUpdateParamsResponse(
    [JsonProperty("dispenserProviderUpdateParams_collection")]
    ICollection<DispenserProviderUpdateParams> DispenserUpdateParams
);