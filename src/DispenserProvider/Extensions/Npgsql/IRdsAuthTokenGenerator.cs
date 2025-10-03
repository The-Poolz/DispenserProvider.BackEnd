using Amazon;

namespace DispenserProvider.Extensions.Npgsql;

public interface IRdsAuthTokenGenerator
{
    public string GenerateAuthToken(RegionEndpoint region, string hostname, int port, string dbUser);
}