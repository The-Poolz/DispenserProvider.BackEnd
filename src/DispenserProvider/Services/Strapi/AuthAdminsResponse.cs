using Newtonsoft.Json;
using Poolz.Finance.CSharp.Strapi;

namespace DispenserProvider.Services.Strapi;

[method: JsonConstructor]
public record AuthAdminsResponse(
    [JsonProperty("authAdministrators")] IReadOnlyCollection<AuthAdministrator> Admins,
    [JsonProperty("authRoles")] IReadOnlyCollection<AuthAdministrator> Roles,
    [JsonProperty("chains")] IReadOnlyCollection<Chain> Chains
);