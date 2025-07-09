using GraphQL.Client.Http;
using System.Net.Http.Headers;
using DispenserProvider.Extensions;
using EnvironmentManager.Extensions;
using Poolz.Finance.CSharp.TheGraph;
using DispenserProvider.Services.Strapi;
using GraphQL.Client.Serializer.Newtonsoft;
using DispenserProvider.Services.TheGraph.Models;

namespace DispenserProvider.Services.TheGraph;

public class TheGraphClient(IStrapiClient strapi) : ITheGraphClient
{
    public async Task<ICollection<DispenserUpdateParams>> GetDispenserUpdateParamsAsync(long chainId, int page, int limit)
    {
        var theGraphUrl = strapi.ReceiveTheGraphUrl(chainId);
        var theGraphClient = new GraphQLHttpClient(theGraphUrl, new NewtonsoftJsonSerializer(), new HttpClient
        {
            DefaultRequestHeaders = {
                Authorization = new AuthenticationHeaderValue("Bearer", Env.THE_GRAPH_AUTHORIZATION_TOKEN.GetRequired())
            }
        });

        var all = new List<DispenserProviderUpdateParams>();
        var skip = (page - 1) * limit;
        while (true)
        {
            var firstFilter = new GraphQlQueryParameter<int?>("firstFilter", limit);
            var skipFilter = new GraphQlQueryParameter<int?>("skipFilter", skip);
            var query = new QueryQueryBuilder()
                .WithDispenserProviderUpdateParamsCollection(new DispenserProviderUpdateParamsQueryBuilder()
                    .WithPoolId()
                    .WithParams(),
                    subgraphError: SubgraphErrorPolicy.Allow,
                    first: firstFilter,
                    skip: skipFilter
                )
                .WithParameter(firstFilter)
                .WithParameter(skipFilter)
                .Build();

            var response = await theGraphClient.SendQueryAsync<DispenserUpdateParamsResponse>(new GraphQLHttpRequest(query));
            var data = response.EnsureNoErrors();

            if (data.DispenserUpdateParams.Count == 0) break;

            all.AddRange(data.DispenserUpdateParams);

            if (data.DispenserUpdateParams.Count < limit) break;

            skip += limit;
        }

        return all
            .Select(x => new DispenserUpdateParams(
                PoolId: x.PoolId!.Value,
                WeiStartAmount: x.Params.First().ToString()
            ))
            .ToArray();
    }
}