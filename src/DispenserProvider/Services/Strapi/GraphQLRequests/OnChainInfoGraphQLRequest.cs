using Poolz.Finance.CSharp.Strapi;

namespace DispenserProvider.Services.Strapi.GraphQLRequests;

public static class OnChainInfoGraphQLRequest
{
    public const string NameOfLockDealNFT = "LockDealNFT";
    public const string NameOfDispenserProvider = "DispenserProvider";

    public static QueryQueryBuilder CreateQueryBuilder(long chainId)
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
        var statusFilter = new GraphQlQueryParameter<PublicationStatus?>("status", "PublicationStatus", PublicationStatus.Published);

        return new QueryQueryBuilder()
            .WithChains(new ChainQueryBuilder()
                .WithContractsOnChain(new ContractsOnChainQueryBuilder()
                    .WithRpc()
                    .WithContracts(new ComponentContractOnChainContractOnChainQueryBuilder()
                        .WithAddress()
                        .WithContractVersion(new ContractQueryBuilder()
                            .WithNameVersion()
                        ),
                        contractsFilter
                    )
                ),
                chainFilter,
                status: statusFilter
            )
            .WithParameter(contractsFilter)
            .WithParameter(chainFilter)
            .WithParameter(statusFilter);
    }
}