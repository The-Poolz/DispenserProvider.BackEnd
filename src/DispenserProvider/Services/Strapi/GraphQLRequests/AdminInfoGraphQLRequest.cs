using Net.Web3.EthereumWallet;
using Poolz.Finance.CSharp.Strapi;
using EnvironmentManager.Extensions;

namespace DispenserProvider.Services.Strapi.GraphQLRequests;

public static class AdminInfoGraphQLRequest
{
    public static QueryQueryBuilder CreateQueryBuilder(EthereumAddress userAddress, IReadOnlyCollection<long> chainIDs)
    {
        var adminsFilter = new GraphQlQueryParameter<AuthAdministratorFiltersInput>("adminsFilter", new AuthAdministratorFiltersInput
        {
            Wallet = new StringFilterInput { Eq = userAddress.Address }
        });
        var rolesFilter = new GraphQlQueryParameter<AuthRoleFiltersInput>("rolesFilter", new AuthRoleFiltersInput
        {
            Name = new StringFilterInput { Eq = Env.ROLE_NAME_OF_DISPENSER_DEV.GetRequired() },
            UserIDs = new AuthUserFiltersInput
            {
                Wallet = new StringFilterInput { Eq = userAddress.Address }
            }
        });
        var chainsFilter = new GraphQlQueryParameter<ChainFiltersInput>("chainsFilter", new ChainFiltersInput
        {
            IsTest = new BooleanFilterInput { Eq = true },
            ChainId = new LongFilterInput { In = chainIDs.Select(x => (long?)x).ToList() }
        });
        var statusFilter = new GraphQlQueryParameter<PublicationStatus?>("status", "PublicationStatus", PublicationStatus.Published);

        return new QueryQueryBuilder()
            .WithAuthAdministrators(new AuthAdministratorQueryBuilder()
                .WithDocumentId(),
                adminsFilter,
                status: statusFilter
            )
            .WithAuthRoles(new AuthRoleQueryBuilder()
                .WithDocumentId(),
                rolesFilter,
                status: statusFilter
            )
            .WithChains(new ChainQueryBuilder()
                .WithDocumentId(),
                chainsFilter
            )
            .WithParameter(adminsFilter)
            .WithParameter(rolesFilter)
            .WithParameter(chainsFilter)
            .WithParameter(statusFilter);
    }
}