using AuthDB;
using CovalentDb;
using SecretsManager;
using FluentValidation;
using DispenserProvider.Options;
using DispenserProvider.DataBase;
using EnvironmentManager.Extensions;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Web3;
using Net.Utils.ErrorHandler.Extensions;
using ConfiguredSqlConnection.Extensions;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Handlers;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.MessageTemplate.Services;
using DispenserProvider.MessageTemplate.Validators;
using DispenserProvider.Services.Handlers.ReadAsset;
using DispenserProvider.Services.Handlers.CreateAsset;
using DispenserProvider.Services.Handlers.DeleteAsset;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Handlers.ListOfAssets;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.MessageTemplate.Models.Validators;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DispenserProvider.Services.Handlers.ReadAsset.Models;
using DispenserProvider.Services.Handlers.GenerateSignature;
using DispenserProvider.Services.Handlers.RetrieveSignature;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Validators.Signature.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;

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
                _ => throw ErrorCode.INVALID_STAGE.ToException()
            }
        };
        return services.BuildServiceProvider();
    }

    private static IServiceCollection Default => new ServiceCollection()
        .AddScoped<IValidator<CreateValidatorSettings>, CreateValidator>()
        .AddScoped<IValidator<DeleteValidatorSettings>, DeleteValidator>()
        .AddScoped<IAdminValidationService, AdminValidationService>()
        .AddScoped<IValidator<GenerateSignatureValidatorRequest>, GenerateSignatureValidator>()
        .AddScoped<IValidator<RetrieveSignatureValidatorRequest>, RetrieveSignatureRequestValidator>()
        .AddScoped<IValidator<PoolOwnershipValidatorRequest>, PoolOwnershipValidator>()
        .AddScoped<UpdatingSignatureValidator>()
        .AddScoped<RefundSignatureValidator>()
        .AddScoped<AssetAvailabilityValidator>()
        .AddScoped<IDispenserManager, DispenserManager>()
        .AddScoped<SecretManager>()
        .AddScoped<ISignatureGenerator, SignatureGenerator>()
        .AddScoped<ISignatureProcessor, SignatureProcessor>()
        .AddScoped<IChainProvider, ChainProvider>()
        .AddScoped<ILockDealNFTContract, LockDealNFTContract>()
        .AddScoped<IDispenserProviderContract, DispenserProviderContract>()
        .AddScoped<ITakenTrackManager, TakenTrackManager>()
        .AddScoped<IRequestHandler<CreateAssetRequest, CreateAssetResponse>, CreateAssetHandler>()
        .AddScoped<IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>, DeleteAssetHandler>()
        .AddScoped<IRequestHandler<ReadAssetRequest, ReadAssetResponse>, ReadAssetHandler>()
        .AddScoped<IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>, ListOfAssetsHandler>()
        .AddScoped<IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>, GenerateSignatureHandler>()
        .AddScoped<IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>, RetrieveSignatureHandler>()
        .AddScoped<IHandlerFactory, HandlerFactory>();

    private static IServiceCollection Prod => new ServiceCollection()
        .AddDbContextFactory<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddScoped<ISignerManager, SignerManager>();

    private static IServiceCollection Stage => new ServiceCollection()
        .AddDbContextFactory<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "DispenserStage")))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "AuthStage")))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "DownloaderStage")))
        .AddScoped<ISignerManager, EnvSignerManager>();
}