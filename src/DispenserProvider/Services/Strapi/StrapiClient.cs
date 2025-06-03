using GraphQL;
using GraphQL.Client.Http;
using Poolz.Finance.CSharp.Strapi;
using EnvironmentManager.Extensions;
using Net.Utils.ErrorHandler.Extensions;
using GraphQL.Client.Serializer.Newtonsoft;
using DispenserProvider.Services.Strapi.Models;
using DispenserProvider.MessageTemplate.Services;
using DispenserProvider.Services.Strapi.GraphQLRequests;

namespace DispenserProvider.Services.Strapi;

public class StrapiClient : IStrapiClient, IAdminValidationService
{
    public static readonly string ApiUrl = Env.STRAPI_GRAPHQL_URL.GetRequired<string>();

    private readonly GraphQLHttpClient _client = new(
        new GraphQLHttpClientOptions { EndPoint = new Uri(ApiUrl) },
        new NewtonsoftJsonSerializer()
    );

    public OnChainInfo ReceiveChainInfo(long chainId)
    {
        var queryBuilder = OnChainInfoGraphQLRequest.CreateQueryBuilder(chainId);

        var response = _client.SendQueryAsync<OnChainInfoResponse>(new GraphQLRequest 
        {
            Query = queryBuilder.Build()
        }).GetAwaiter().GetResult();

        EnsureNoGraphQLErrors(response);

        if (!response.Data.Chains.Any() || response.Data.Chains.First().ContractsOnChain == null)
        {
            throw ErrorCode.CHAIN_NOT_SUPPORTED.ToException(new
            {
                ChainId = chainId
            });
        }

        var chain = response.Data.Chains.First();
        var dispenserProvider = ExtractContractAddress(chain, OnChainInfoGraphQLRequest.NameOfDispenserProvider, chainId, ErrorCode.DISPENSER_PROVIDER_NOT_SUPPORTED);
        var lockDealNFT = ExtractContractAddress(chain, OnChainInfoGraphQLRequest.NameOfLockDealNFT, chainId, ErrorCode.LOCK_DEAL_NFT_NOT_SUPPORTED);

        return new OnChainInfo(chain.ContractsOnChain.Rpc, dispenserProvider, lockDealNFT);
    }

    public bool IsValidAdmin(string userAddress, IReadOnlyCollection<long> chainIDs)
    {
        var queryBuilder = AdminInfoGraphQLRequest.CreateQueryBuilder(userAddress, chainIDs);

        var response = _client.SendQueryAsync<AuthAdminsResponse>(new GraphQLRequest
        {
            Query = queryBuilder.Build()
        }).GetAwaiter().GetResult();

        EnsureNoGraphQLErrors(response);

        return response.Data.Admins.Any() || (response.Data.Roles.Any() && response.Data.Chains.Count == chainIDs.Count);
    }

    private static void EnsureNoGraphQLErrors<TData>(GraphQLResponse<TData> response)
    {
        if (response.Errors != null && response.Errors.Any())
        {
            var errorMessage = string.Join(Environment.NewLine, response.Errors.Select(x => x.Message));
            throw new InvalidOperationException(errorMessage);
        }
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