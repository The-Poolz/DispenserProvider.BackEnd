using DispenserProvider.Extensions;
using GraphQL;
using GraphQL.Client.Http;
using Poolz.Finance.CSharp.Strapi;
using EnvironmentManager.Extensions;
using Net.Utils.ErrorHandler.Extensions;
using GraphQL.Client.Serializer.Newtonsoft;
using DispenserProvider.MessageTemplate.Services;

namespace DispenserProvider.Services.Strapi;

public class StrapiClient : IStrapiClient, IAdminValidationService
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

        var data = response.EnsureNoErrors();

        if (!data.Chains.Any() || data.Chains.First().ContractsOnChain == null)
        {
            throw ErrorCode.CHAIN_NOT_SUPPORTED.ToException(new
            {
                ChainId = chainId
            });
        }

        var chain = data.Chains.First();
        var dispenserProvider = ExtractContractAddress(chain.ContractsOnChain.Contracts, NameOfDispenserProvider, chainId, ErrorCode.DISPENSER_PROVIDER_NOT_SUPPORTED);
        var lockDealNFT = ExtractContractAddress(chain.ContractsOnChain.Contracts, NameOfLockDealNFT, chainId, ErrorCode.LOCK_DEAL_NFT_NOT_SUPPORTED);

        return new OnChainInfo(chain.ContractsOnChain.Rpc, dispenserProvider, lockDealNFT);
    }

    public string ReceiveTheGraphUrl(long chainId)
    {
        var contractsFilter = new GraphQlQueryParameter<ComponentContractOnChainContractOnChainFiltersInput>("contractsFilter", new ComponentContractOnChainContractOnChainFiltersInput
        {
            ContractVersion = new ContractFiltersInput
            {
                NameVersion = new StringFilterInput { Contains = NameOfDispenserProvider }
            }
        });
        var chainFilter = new GraphQlQueryParameter<ContractsOnChainFiltersInput>("chainFilter", new ContractsOnChainFiltersInput
        {
            Chain = new ChainFiltersInput
            {
                ChainId = new LongFilterInput { Eq = chainId }
            }
        });
        var statusParam = new GraphQlQueryParameter<PublicationStatus?>("status", "PublicationStatus", PublicationStatus.Published);

        var query = new QueryQueryBuilder()
            .WithContractsOnChains(new ContractsOnChainQueryBuilder()
                .WithTheGraphUrl()
                .WithContracts(new ComponentContractOnChainContractOnChainQueryBuilder()
                    .WithAddress(),
                    filters: contractsFilter
                ),
                filters: chainFilter,
                status: statusParam
            )
            .WithParameter(contractsFilter)
            .WithParameter(chainFilter)
            .WithParameter(statusParam)
            .Build();

        var response = _client.SendQueryAsync<TheGraphDataResponse>(new GraphQLRequest(query)).GetAwaiter().GetResult();

        var data = response.EnsureNoErrors();

        if (!data.ContractsOnChain.Any())
        {
            throw ErrorCode.CHAIN_NOT_SUPPORTED.ToException(new
            {
                ChainId = chainId
            });
        }

        var contractsOnChain = data.ContractsOnChain.First();

        return contractsOnChain.TheGraphUrl;
    }

    public bool IsValidAdmin(string userAddress)
    {
        var adminsFilter = new GraphQlQueryParameter<AuthAdministratorFiltersInput>("adminsFilter", new AuthAdministratorFiltersInput
        {
            Wallet = new StringFilterInput { Eq = userAddress }
        });

        var queryBuilder = new QueryQueryBuilder()
            .WithAuthAdministrators(
                new AuthAdministratorQueryBuilder().WithDocumentId(),
                adminsFilter
            )
            .WithParameter(adminsFilter);

        var response = _client.SendQueryAsync<AuthAdminsResponse>(new GraphQLRequest
        {
            Query = queryBuilder.Build()
        }).GetAwaiter().GetResult();

        var data = response.EnsureNoErrors();

        return data.Admins.Any();
    }

    private static string ExtractContractAddress(ICollection<ComponentContractOnChainContractOnChain> contracts, string nameOfContract, long chainId, ErrorCode error)
    {
        var contract = contracts.FirstOrDefault(x =>
            x.ContractVersion.NameVersion.Contains(nameOfContract)
        );
        if (contract == null) throw error.ToException(new
        {
            ChainId = chainId
        });
        return contract.Address;
    }
}