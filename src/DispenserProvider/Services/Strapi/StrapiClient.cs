using GraphQL;
using GraphQL.Client.Http;
using Poolz.Finance.CSharp.Strapi;
using EnvironmentManager.Extensions;
using GraphQL.Client.Serializer.Newtonsoft;

namespace DispenserProvider.Services.Strapi;

public class StrapiClient : IStrapiClient
{
    public static readonly string ApiUrl = Env.STRAPI_GRAPHQL_URL.GetRequired<string>();

    private readonly GraphQLHttpClient _client = new(
        new GraphQLHttpClientOptions { EndPoint = new Uri(ApiUrl) },
        new NewtonsoftJsonSerializer()
    );

    public Chain ReceiveChainInfo(long chainId)
    {
        var contractsFilter = new GraphQlQueryParameter<ComponentContractOnChainContractOnChainFiltersInput>("contractsFilter", new ComponentContractOnChainContractOnChainFiltersInput
        {
            Or = new[]
            {
                new ComponentContractOnChainContractOnChainFiltersInput
                {
                    ContractVersion = new ContractFiltersInput
                    {
                        NameVersion = new StringFilterInput { Contains = "LockDealNFT" }
                    }
                },
                new ComponentContractOnChainContractOnChainFiltersInput
                {
                    ContractVersion = new ContractFiltersInput
                    {
                        NameVersion = new StringFilterInput { Contains = "DispenserProvider" }
                    }
                }
            }
        });
        var chainFilter = new GraphQlQueryParameter<ChainFiltersInput>("chainFilter", new ChainFiltersInput
        {
            ChainId = new LongFilterInput { Eq = chainId }
        });
        var statusParam = new GraphQlQueryParameter<PublicationStatus?>("status", "PublicationStatus", PublicationStatus.Published);

        var queryBuilder = new QueryQueryBuilder()
            .WithChains(
                new ChainQueryBuilder()
                    .WithContractsOnChain(
                        new ContractsOnChainQueryBuilder()
                            .WithRpc()
                            .WithContracts(
                                new ComponentContractOnChainContractOnChainQueryBuilder()
                                    .WithContractVersion(
                                        new ContractQueryBuilder().WithNameVersion()
                                    )
                                    .WithAddress(),
                                    contractsFilter
                            )
                    ),
                chainFilter,
                status: statusParam
            )
            .WithParameter(contractsFilter)
            .WithParameter(chainFilter)
            .WithParameter(statusParam);

        var query = queryBuilder.Build();
        
        var response = _client.SendQueryAsync<OnChainInfo>(new GraphQLRequest 
        {
            Query = query
        }).GetAwaiter().GetResult();

        return response.Data.Chains.First();
    }
}