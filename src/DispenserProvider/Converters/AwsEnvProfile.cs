using Amazon;

namespace DispenserProvider.Converters;

public sealed class AwsEnvProfile : AutoMapper.Profile
{
    public AwsEnvProfile()
    {
        CreateMap<string, RegionEndpoint>()
            .ConvertUsing<StringToRegionEndpointConverter>();
    }
}