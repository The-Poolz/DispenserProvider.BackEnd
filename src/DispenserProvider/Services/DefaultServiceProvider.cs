﻿using AuthDB;
using CovalentDb;
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
using MediatR.Extensions.FluentValidation.AspNetCore;
using DispenserProvider.Services.Validators.AdminRequest;
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
        .AddScoped<IAdminValidationService, AdminValidationService>()
        .AddScoped<IDispenserManager, DispenserManager>()
        .AddScoped<ISignatureGenerator, SignatureGenerator>()
        .AddScoped<ISignatureProcessor, SignatureProcessor>()
        .AddScoped<IChainProvider, ChainProvider>()
        .AddScoped<ILockDealNFTContract, LockDealNFTContract>()
        .AddScoped<IDispenserProviderContract, DispenserProviderContract>()
        .AddScoped<IBuilderContract, BuilderContract>()
        .AddScoped<ITakenTrackManager, TakenTrackManager>();

    private static IServiceCollection Prod => new ServiceCollection()
        .AddDbContextFactory<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnectionFromSecret(Env.SECRET_NAME_OF_DISPENSER_CONNECTION.ToString())))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnectionFromSecret(Env.SECRET_NAME_OF_AUTH_CONNECTION.ToString())))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnectionFromSecret(Env.SECRET_NAME_OF_DOWNLOADER_CONNECTION.ToString())))
        .AddScoped<ISignerManager, SignerManager>();

    private static IServiceCollection Stage => new ServiceCollection()
        .AddDbContextFactory<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnectionFromConfiguration("DispenserStage")))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnectionFromConfiguration("AuthStage")))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnectionFromConfiguration("DownloaderStage")))
        .AddScoped<ISignerManager, SignerManager>();
}