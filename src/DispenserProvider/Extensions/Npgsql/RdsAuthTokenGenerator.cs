using Amazon;
using Amazon.RDS.Util;

namespace DispenserProvider.Extensions.Npgsql;

public class RdsAuthTokenGenerator : IRdsAuthTokenGenerator
{
    public string GenerateAuthToken(RegionEndpoint region, string hostname, int port, string dbUser)
    {
        return RDSAuthTokenGenerator.GenerateAuthToken(region, hostname, port, dbUser);
    }
}