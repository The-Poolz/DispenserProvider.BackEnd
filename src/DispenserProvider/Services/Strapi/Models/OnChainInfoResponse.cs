using Newtonsoft.Json;
using Poolz.Finance.CSharp.Strapi;

namespace DispenserProvider.Services.Strapi.Models;

[method: JsonConstructor]
public record OnChainInfoResponse([JsonProperty("chains")] IReadOnlyCollection<Chain> Chains);