using AuthDB;
using CovalentDb;
using SecretsManager;
using FluentValidation;
using Net.Web3.EthereumWallet;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using TokenSchedule.FluentValidation;
using ConfiguredSqlConnection.Extensions;
using DispenserProvider.Services.Handlers;
using TokenSchedule.FluentValidation.Models;
using Microsoft.Extensions.DependencyInjection;
using DispenserProvider.Services.Handlers.ReadAsset;
using DispenserProvider.Services.Handlers.CreateAsset;
using DispenserProvider.Services.Handlers.DeleteAsset;
using DispenserProvider.Services.Handlers.ListOfAssets;
using DispenserProvider.Services.Validators.AdminRequest;
using DispenserProvider.Services.Handlers.ReadAsset.Models;
using DispenserProvider.Services.Handlers.GenerateSignature;
using DispenserProvider.Services.Handlers.RetrieveSignature;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using DispenserProvider.Services.Handlers.DeleteAsset.Models;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;
using DispenserProvider.Services.Validators.GenerateSignature;
using DispenserProvider.Services.Validators.AdminRequest.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Web3;
using DispenserProvider.Services.Handlers.GenerateSignature.Models;
using DispenserProvider.Services.Handlers.RetrieveSignature.Models;
using DispenserProvider.Services.Handlers.GenerateSignature.Helpers;
using DispenserProvider.Services.Validators.GenerateSignature.Models;

namespace DispenserProvider.Services;

public static class DefaultServiceProvider
{
    public static IServiceProvider Default => new ServiceCollection()
        .AddDbContext<DispenserContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "DispenserStage")))
        .AddDbContext<AuthContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "AuthStage")))
        .AddDbContext<CovalentContext>(options => options.UseSqlServer(ConnectionStringFactory.GetConnection(ContextOption.Staging, "DownloaderStage")))
        .AddScoped<IValidator<AdminValidationRequest<CreateAssetMessage>>, AdminRequestValidator<CreateAssetMessage>>()
        .AddScoped<IValidator<AdminValidationRequest<DeleteAssetMessage>>, AdminRequestValidator<DeleteAssetMessage>>()
        .AddScoped<IValidator<IEnumerable<EthereumAddress>>, OrderedUsersValidator>()
        .AddScoped<IValidator<GenerateSignatureValidatorRequest>, GenerateSignatureValidator>()
        .AddScoped<IValidator<IEnumerable<IValidatedScheduleItem>>, ScheduleValidator>()
        .AddScoped<UpdatingSignatureValidator>()
        .AddScoped<RefundSignatureValidator>()
        .AddScoped<AssetAvailabilityValidator>()
        .AddScoped<SecretManager>()
        .AddScoped<SignatureGenerator>()
        .AddScoped<ChainProvider>()
        .AddScoped<SignatureProcessor>()
        .AddScoped<DispenserProviderContract>()
        .AddScoped<IRequestHandler<CreateAssetRequest, CreateAssetResponse>, CreateAssetHandler>()
        .AddScoped<IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>, DeleteAssetHandler>()
        .AddScoped<IRequestHandler<ReadAssetRequest, ReadAssetResponse>, ReadAssetHandler>()
        .AddScoped<IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>, ListOfAssetsHandler>()
        .AddScoped<IRequestHandler<GenerateSignatureRequest, GenerateSignatureResponse>, GenerateSignatureHandler>()
        .AddScoped<IRequestHandler<RetrieveSignatureRequest, RetrieveSignatureResponse>, RetrieveSignatureHandler>()
        .AddScoped<IHandlerFactory, HandlerFactory>()
        .BuildServiceProvider();
}