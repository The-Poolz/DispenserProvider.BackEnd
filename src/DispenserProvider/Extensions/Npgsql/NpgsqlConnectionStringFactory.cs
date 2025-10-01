using Npgsql;
using Amazon;
using SecretsManager;
using Amazon.RDS.Util;
using EnvironmentManager.Core;
using DispenserProvider.Converters;
using EnvironmentManager.Extensions;

namespace DispenserProvider.Extensions.Npgsql;

public static class NpgsqlConnectionStringFactory
{
    public static string BuildProd()
    {
        var secretManager = new SecretManager();
        var secretId = Env.PROD_POSTGRES_SECRET_ID.GetRequired();
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Env.PROD_POSTGRES_HOSTNAME.GetRequired(),
            Port = Env.PROD_POSTGRES_PORT.GetRequired<int>(),
            Database = Env.PROD_POSTGRES_DB_NAME.GetRequired(),
            Username = secretManager.GetSecretValue(secretId, Env.PROD_POSTGRES_SECRET_KEY_OF_USERNAME.GetRequired()),
            Password = secretManager.GetSecretValue(secretId, Env.PROD_POSTGRES_SECRET_KEY_OF_PASSWORD.GetRequired()),
            Timeout = 5,
            IncludeErrorDetail = false
        };
        return builder.ConnectionString;
    }

    public static string BuildStage()
    {
        var hostname = Env.STAGE_POSTGRES_HOSTNAME.GetRequired();
        var port = Env.STAGE_POSTGRES_PORT.GetRequired<int>();
        var dbUser = Env.STAGE_POSTGRES_DB_USER.GetRequired();

        var envManager = new EnvManager(MapperConfigurationsExtensions.WithAwsRegionConverters());
        var region = envManager.Get<RegionEndpoint?>(nameof(Env.STAGE_POSTGRES_AWS_REGION)) ?? RegionEndpoint.EUCentral1;
        var password = RDSAuthTokenGenerator.GenerateAuthToken(region, hostname, port, dbUser);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = hostname,
            Port = port,
            Database = Env.STAGE_POSTGRES_DB_NAME.GetRequired(),
            Username = dbUser,
            Password = password,
            SslMode = SslMode.VerifyFull,
            SslCertificate = Env.STAGE_POSTGRES_SSL_CERT_FULL_PATH.GetRequired(),
            Timeout = 5,
            IncludeErrorDetail = true
        };
        return builder.ConnectionString;
    }
}