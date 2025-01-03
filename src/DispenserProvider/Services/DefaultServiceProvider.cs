using AuthDB;
using CovalentDb;
using SecretsManager;
using FluentValidation;
using Net.Web3.EthereumWallet;
using DispenserProvider.Options;
using DispenserProvider.DataBase;
using EnvironmentManager.Extensions;
using Microsoft.EntityFrameworkCore;
using TokenSchedule.FluentValidation;
using DispenserProvider.DataBase.Models;
using ConfiguredSqlConnection.Extensions;
using DispenserProvider.Services.Handlers;
using DispenserProvider.Services.Database;
using TokenSchedule.FluentValidation.Models;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Handlers.ReadAsset;
using DispenserProvider.Services.Handlers.CreateAsset;
using DispenserProvider.Services.Handlers.DeleteAsset;
using DispenserProvider.Services.Handlers.ListOfAssets;
using DispenserProvider.Services.Validators.AdminRequest;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DispenserProvider.Services.Handlers.ReadAsset.Models;
using DispenserProvider.Services.Handlers.GenerateSignature;
using DispenserProvider.Services.Handlers.RetrieveSignature;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;
using DispenserProvider.Services.Validators.GenerateSignature;
using DispenserProvider.Services.Validators.RetrieveSignature;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Helpers;
using DispenserProvider.Services.Validators.GenerateSignature.Models;

namespace DispenserProvider.Services;

public static class DefaultServiceProvider
{
    public static IServiceProvider Build()
    {
        var mode = Env.PRODUCTION_MODE.GetRequired<ProductionMode>();
        var services = new ServiceCollection
        {
            Default,
            mode switch
            {
                ProductionMode.Prod => Prod,
                ProductionMode.Stage => Stage,
                _ => throw new NotImplementedException($"No one valid stage in '{nameof(Env.PRODUCTION_MODE)}' environment variable found.")
            }
        };
        return services.BuildServiceProvider();
    }

    private static IServiceCollection Default => new ServiceCollection()
        .AddScoped<IValidator<AdminValidationRequest<CreateAssetMessage>>, AdminRequestValidator<CreateAssetMessage>>()
        .AddScoped<IValidator<AdminValidationRequest<DeleteAssetMessage>>, AdminRequestValidator<DeleteAssetMessage>>()
        .AddScoped<IValidator<IEnumerable<EthereumAddress>>, OrderedUsersValidator>()
        .AddScoped<IValidator<GenerateSignatureValidatorRequest>, GenerateSignatureValidator>()
        .AddScoped<IValidator<IEnumerable<IValidatedScheduleItem>>, ScheduleValidator>()
        .AddScoped<IValidator<DispenserDTO>, RetrieveSignatureRequestValidator>()
        .AddScoped<UpdatingSignatureValidator>()
        .AddScoped<RefundSignatureValidator>()
        .AddScoped<AssetAvailabilityValidator>()
        .AddScoped<SecretManager>()
        .AddScoped<ISignatureProcessor, SignatureProcessor>()
        .AddScoped<IChainProvider, ChainProvider>()
        .AddScoped<IDispenserProviderContract, DispenserProviderContract>()
        .AddScoped<IRequestHandler<CreateAssetRequest, CreateAssetResponse>, CreateAssetHandler>()
        .AddScoped<IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>, DeleteAssetHandler>()
        .AddScoped<IRequestHandler<ReadAssetRequest, ReadAssetResponse>, ReadAssetHandler>()
        .AddScoped<IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>, ListOfAssetsHandler>()
        .AddScoped<IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>, GenerateSignatureHandler>()
        .AddScoped<IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>, RetrieveSignatureHandler>()
        .AddScoped<IHandlerFactory, HandlerFactory>();

    private static IServiceCollection Prod => new ServiceCollection()
        .AddDbContext<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddScoped<ISignerManager, SignerManager>();

    private static IServiceCollection Stage => new ServiceCollection()
        .AddDbContext<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "DispenserStage")))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "AuthStage")))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "DownloaderStage")))
        .AddScoped<ISignerManager, EnvSignerManager>();
}