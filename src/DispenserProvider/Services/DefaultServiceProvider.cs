using AuthDB;
using CovalentDb;
using SecretsManager;
using FluentValidation;
using System.Reflection;
using DispenserProvider.Options;
using DispenserProvider.DataBase;
using EnvironmentManager.Extensions;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Web3;
using Net.Utils.ErrorHandler.Extensions;
using ConfiguredSqlConnection.Extensions;
using DispenserProvider.Services.Database;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Web3.Contracts;
using DispenserProvider.MessageTemplate.Services;
using DispenserProvider.MessageTemplate.Validators;
using DispenserProvider.Services.Validators.Signature;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.MessageTemplate.Models.Validators;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DispenserProvider.Services.Validators.Signature.Models;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;

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
        .AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
        .AddScoped<IValidator<CreateValidatorSettings>, CreateValidator>()
        .AddScoped<IValidator<DeleteValidatorSettings>, DeleteValidator>()
        .AddScoped<IAdminValidationService, AdminValidationService>()
        .AddScoped<IValidator<GenerateSignatureValidatorRequest>, GenerateSignatureValidator>()
        .AddScoped<IValidator<RetrieveSignatureValidatorRequest>, RetrieveSignatureRequestValidator>()
        .AddScoped<IValidator<PoolOwnershipValidatorRequest>, PoolOwnershipValidator>()
        .AddScoped<IValidator<BuildersValidatorRequest>, BuildersValidator>()
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
        .AddScoped<IBuilderContract, BuilderContract>()
        .AddScoped<ITakenTrackManager, TakenTrackManager>();

    private static IServiceCollection Prod => new ServiceCollection()
        .AddDbContextFactory<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Prod)))
        .AddScoped<ISignerManager, SignerManager>();

    private static IServiceCollection Stage => new ServiceCollection()
        .AddDbContextFactory<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "DispenserStage")))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "AuthStage")))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "DownloaderStage")))
        .AddScoped<ISignerManager, SignerManager>();
}