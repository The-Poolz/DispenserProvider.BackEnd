using Newtonsoft.Json;
using Poolz.Finance.CSharp.Strapi;

namespace DispenserProvider.Services.Strapi;

[method: JsonConstructor]
public record TheGraphDataResponse([JsonProperty("contractsOnChains")] ICollection<ContractsOnChain> ContractsOnChain);