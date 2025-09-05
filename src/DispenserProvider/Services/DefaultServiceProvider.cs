using Amazon;
using SecretsManager;
using Amazon.RDS.Util;
using FluentValidation;
using System.Reflection;
using EnvironmentManager.Core;
using DispenserProvider.Options;
using DispenserProvider.DataBase;
using DispenserProvider.Converters;
using EnvironmentManager.Extensions;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Web3;
using DispenserProvider.Services.Strapi;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.Services.Database;
using DispenserProvider.Services.Resilience;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Web3.Contracts;
using DispenserProvider.Services.Web3.MultiCall;
using DispenserProvider.MessageTemplate.Services;
using DispenserProvider.MessageTemplate.Validators;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        .AddFluentValidation([Assembly.GetExecutingAssembly()])
        .AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly(), typeof(CreateValidator).GetTypeInfo().Assembly])
        .AddScoped<IRetryExecutor, RetryExecutor>()
        .AddScoped<IAdminValidationService, StrapiClient>()
        .AddScoped<IDispenserManager, DispenserManager>()
        .AddScoped<ISignatureGenerator, SignatureGenerator>()
        .AddScoped<ISignatureProcessor, SignatureProcessor>()
        .AddScoped<IChainProvider, ChainProvider>()
        .AddScoped<ILockDealNFTContract, LockDealNFTContract>()
        .AddScoped<IDispenserProviderContract, DispenserProviderContract>()
        .AddScoped<IBuilderContract, BuilderContract>()
        .AddScoped<ITakenTrackManager, TakenTrackManager>()
        .AddScoped<IMultiCallContract, MultiCallContract>()
        .AddScoped<IStrapiClient, StrapiClient>();

    private static IServiceCollection Prod => new ServiceCollection()
        .AddDbContextFactory<DispenserContext>(options =>
        {
            var hostname = Env.PROD_POSTGRES_HOSTNAME.GetRequired();
            var port = Env.PROD_POSTGRES_PORT.GetRequired<int>();
            var dbName = Env.PROD_POSTGRES_DB_NAME.GetRequired();
            var secretManager = new SecretManager();
            var secretId = Env.PROD_POSTGRES_SECRET_ID.GetRequired();
            var dbUser = secretManager.GetSecretValue(secretId, Env.PROD_POSTGRES_SECRET_KEY_OF_USERNAME.GetRequired());
            var dbPassword = secretManager.GetSecretValue(secretId, Env.PROD_POSTGRES_SECRET_KEY_OF_PASSWORD.GetRequired());

            var connectionString = $"Server={hostname};Port={port};User Id={dbUser};Password={dbPassword};Database={dbName};";

            options.UseNpgsql(connectionString);
        })
        .AddScoped<ISignerManager, SignerManager>();

    private static IServiceCollection Stage => new ServiceCollection()
        .AddDbContextFactory<DispenserContext>(options =>
        {
            var hostname = Env.STAGE_POSTGRES_HOSTNAME.GetRequired();
            var port = Env.STAGE_POSTGRES_PORT.GetRequired<int>();
            var dbUser = Env.STAGE_POSTGRES_DB_USER.GetRequired();
            var dbName = Env.STAGE_POSTGRES_DB_NAME.GetRequired();
            var sslCertPath = Env.STAGE_POSTGRES_SSL_CERT_FULL_PATH.GetRequired();
            var envManager = new EnvManager(MapperConfigurationsExtensions.WithAwsRegionConverters());
            var region = envManager.Get<RegionEndpoint?>(nameof(Env.STAGE_POSTGRES_AWS_REGION)) ?? RegionEndpoint.EUCentral1;

            var pwd = RDSAuthTokenGenerator.GenerateAuthToken(region, hostname, port, dbUser);

            var connectionString = $"Server={hostname};User Id={dbUser};Password={pwd};Database={dbName};SSL Mode=VerifyFull;Root Certificate={sslCertPath};Include Error Detail=true";

            options.UseNpgsql(connectionString);
        })
        .AddScoped<ISignerManager, EnvSignerManager>();
}