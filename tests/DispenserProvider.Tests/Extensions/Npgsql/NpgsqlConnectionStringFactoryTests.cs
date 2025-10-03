using Moq;
using Xunit;
using Npgsql;
using Amazon;
using SecretsManager;
using DispenserProvider.Extensions.Npgsql;
using FluentAssertions;

namespace DispenserProvider.Tests.Extensions.Npgsql;

[Collection("EnvDependent")]
public class NpgsqlConnectionStringFactoryTests
{
    public class BuildProd
    {
        [Fact]
        internal void Should_ReturnValidConnectionString()
        {
            var env = new Dictionary<string, string?>
            {
                ["PROD_POSTGRES_SECRET_ID"] = "prod/secret",
                ["PROD_POSTGRES_HOSTNAME"] = "prod.db.example",
                ["PROD_POSTGRES_PORT"] = "5432",
                ["PROD_POSTGRES_DB_NAME"] = "prod_db",
                ["PROD_POSTGRES_SECRET_KEY_OF_USERNAME"] = "username_key",
                ["PROD_POSTGRES_SECRET_KEY_OF_PASSWORD"] = "password_key",
            };
            using var _ = EnvScope.Apply(env);

            var secretManager = new Mock<SecretManager>(MockBehavior.Strict);
            secretManager
                .Setup(s => s.GetSecretValue("prod/secret", "username_key"))
                .Returns("prod_user");
            secretManager
                .Setup(s => s.GetSecretValue("prod/secret", "password_key"))
                .Returns("prod_pass");

            var cs = NpgsqlConnectionStringFactory.BuildProd(secretManager.Object);

            var b = new NpgsqlConnectionStringBuilder(cs);
            b.Host.Should().Be("prod.db.example");
            b.Port.Should().Be(5432);
            b.Database.Should().Be("prod_db");
            b.Username.Should().Be("prod_user");
            b.Password.Should().Be("prod_pass");
            b.Timeout.Should().Be(5);
            b.IncludeErrorDetail.Should().BeFalse();

            secretManager.Verify(s => s.GetSecretValue("prod/secret", "username_key"), Times.Once);
            secretManager.Verify(s => s.GetSecretValue("prod/secret", "password_key"), Times.Once);
            secretManager.VerifyNoOtherCalls();
        }
    }

    public class BuildStage
    {
        [Fact]
        internal void When_IamAuthAndDefaults_Should_ReturnValidConnectionString_And_UseDefaultRegion()
        {
            var env = new Dictionary<string, string?>
            {
                ["STAGE_POSTGRES_HOSTNAME"] = "stage.db.internal",
                ["STAGE_POSTGRES_PORT"] = "5432",
                ["STAGE_POSTGRES_DB_USER"] = "stage_user",
                ["STAGE_POSTGRES_DB_NAME"] = "stage_db",
                ["STAGE_POSTGRES_SSL_CERT_FULL_PATH"] = "/certs/rds-ca.pem",
                ["STAGE_POSTGRES_AWS_REGION"] = null
            };
            using var _ = EnvScope.Apply(env);

            var token = "signed-rds-auth-token";
            var rds = new Mock<IRdsAuthTokenGenerator>(MockBehavior.Strict);
            rds.Setup(g => g.GenerateAuthToken(RegionEndpoint.EUCentral1, "stage.db.internal", 5432, "stage_user"))
               .Returns(token);

            var cs = NpgsqlConnectionStringFactory.BuildStage(rds.Object);

            var b = new NpgsqlConnectionStringBuilder(cs);
            b.Host.Should().Be("stage.db.internal");
            b.Port.Should().Be(5432);
            b.Database.Should().Be("stage_db");
            b.Username.Should().Be("stage_user");
            b.Password.Should().Be(token);
            b.SslMode.Should().Be(SslMode.VerifyFull);
            b.Timeout.Should().Be(5);
            b.IncludeErrorDetail.Should().BeTrue();

            cs.Should().Contain("/certs/rds-ca.pem");
            cs.Contains("Root Certificate").Should().BeTrue();

            rds.Verify(g => g.GenerateAuthToken(RegionEndpoint.EUCentral1, "stage.db.internal", 5432, "stage_user"), Times.Once);
            rds.VerifyNoOtherCalls();
        }

        [Fact]
        internal void When_RegionSpecified_Should_UseProvidedRegion()
        {
            var env = new Dictionary<string, string?>
            {
                ["STAGE_POSTGRES_HOSTNAME"] = "stage.db.internal",
                ["STAGE_POSTGRES_PORT"] = "5432",
                ["STAGE_POSTGRES_DB_USER"] = "stage_user",
                ["STAGE_POSTGRES_DB_NAME"] = "stage_db",
                ["STAGE_POSTGRES_SSL_CERT_FULL_PATH"] = "/certs/rds-ca.pem",
                ["STAGE_POSTGRES_AWS_REGION"] = "us-east-1"
            };
            using var _ = EnvScope.Apply(env);

            var token = "token-us-east-1";
            var rds = new Mock<IRdsAuthTokenGenerator>(MockBehavior.Strict);
            rds.Setup(g => g.GenerateAuthToken(RegionEndpoint.USEast1, "stage.db.internal", 5432, "stage_user"))
               .Returns(token);

            var cs = NpgsqlConnectionStringFactory.BuildStage(rds.Object);

            var b = new NpgsqlConnectionStringBuilder(cs);
            b.Password.Should().Be(token);

            rds.Verify(g => g.GenerateAuthToken(RegionEndpoint.USEast1, "stage.db.internal", 5432, "stage_user"), Times.Once);
            rds.VerifyNoOtherCalls();
        }
    }
}

[CollectionDefinition("EnvDependent", DisableParallelization = true)]
public class EnvDependentCollection : ICollectionFixture<object>;