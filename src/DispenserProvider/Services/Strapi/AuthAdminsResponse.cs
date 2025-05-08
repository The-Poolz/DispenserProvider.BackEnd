using Newtonsoft.Json;
using Poolz.Finance.CSharp.Strapi;

namespace DispenserProvider.Services.Strapi;

[method: JsonConstructor]
public record AuthAdminsResponse([JsonProperty("authAdministrators")] IEnumerable<AuthAdministrator> Admins);