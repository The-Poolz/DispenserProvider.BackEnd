using Newtonsoft.Json;
using Poolz.Finance.CSharp.Strapi;

namespace DispenserProvider.Services.Strapi;

[method: JsonConstructor]
public record OnChainInfoResponse([JsonProperty("chains")] ICollection<Chain> Chains);