using GraphQL;
using GraphQL.Client.Http;
using Poolz.Finance.CSharp.Strapi;
using EnvironmentManager.Extensions;
using Net.Utils.ErrorHandler.Extensions;
using GraphQL.Client.Serializer.Newtonsoft;

namespace DispenserProvider.Services.Strapi;

public class StrapiClient : IStrapiClient
{
    public const string NameOfLockDealNFT = "LockDealNFT";
    public const string NameOfDispenserProvider = "DispenserProvider";
    public static readonly string ApiUrl = Env.STRAPI_GRAPHQL_URL.GetRequired<string>();

    private readonly GraphQLHttpClient _client = new(
        new GraphQLHttpClientOptions { EndPoint = new Uri(ApiUrl) },
        new NewtonsoftJsonSerializer()
    );

    public OnChainInfo ReceiveChainInfo(long chainId)
    {
        var contractsFilter = new GraphQlQueryParameter<ComponentContractOnChainContractOnChainFiltersInput>("contractsFilter", new ComponentContractOnChainContractOnChainFiltersInput
        {
            Or = new[]
            {
                new ComponentContractOnChainContractOnChainFiltersInput
                {
                    ContractVersion = new ContractFiltersInput
                    {
                        NameVersion = new StringFilterInput { Contains = NameOfLockDealNFT }
                    }
                },
                new ComponentContractOnChainContractOnChainFiltersInput
                {
                    ContractVersion = new ContractFiltersInput
                    {
                        NameVersion = new StringFilterInput { Contains = NameOfDispenserProvider }
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

        var response = _client.SendQueryAsync<OnChainInfoResponse>(new GraphQLRequest 
        {
            Query = queryBuilder.Build()
        }).GetAwaiter().GetResult();

        if (response.Errors != null && response.Errors.Any())
        {
            var errorMessage = string.Join(Environment.NewLine, response.Errors.Select(x => x.Message));
            throw new InvalidOperationException(errorMessage);
        }

        if (!response.Data.Chains.Any())
        {
            throw ErrorCode.CHAIN_NOT_SUPPORTED.ToException(new
            {
                ChainId = chainId
            });
        }

        var chain = response.Data.Chains.First();
        var dispenserProvider = ExtractContractAddress(chain, NameOfDispenserProvider, chainId, ErrorCode.DISPENSER_PROVIDER_NOT_SUPPORTED);
        var lockDealNFT = ExtractContractAddress(chain, NameOfLockDealNFT, chainId, ErrorCode.LOCK_DEAL_NFT_NOT_SUPPORTED);

        return new OnChainInfo(chain.ContractsOnChain.Rpc, dispenserProvider, lockDealNFT);
    }

    private static string ExtractContractAddress(Chain chain, string nameOfContract, long chainId, ErrorCode error)
    {
        var contract = chain.ContractsOnChain.Contracts.FirstOrDefault(x =>
            x.ContractVersion.NameVersion.Contains(nameOfContract)
        );
        if (contract == null) throw error.ToException(new
        {
            ChainId = chainId
        });
        return contract.Address;
    }
}