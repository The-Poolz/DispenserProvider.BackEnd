using Amazon;
using AutoMapper;

namespace DispenserProvider.Converters;

public sealed class StringToRegionEndpointConverter : ITypeConverter<string, RegionEndpoint>
{
    public RegionEndpoint Convert(string source, RegionEndpoint destination, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException(nameof(source), "AWS region is empty. Expected e.g. 'eu-central-1'.");
        }

        var normalized = source.Trim().Replace('_', '-').ToLowerInvariant();

        var endpoint = RegionEndpoint.GetBySystemName(normalized);

        var isKnown = RegionEndpoint.EnumerableAllRegions.Any(r => string.Equals(r.SystemName, endpoint.SystemName, StringComparison.OrdinalIgnoreCase));

        if (!isKnown)
        {
            throw new ArgumentException($"Unknown AWS region '{source}'. Use a system name like 'eu-central-1' or 'us-east-1'.");
        }

        return endpoint;
    }
}