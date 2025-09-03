using AutoMapper;
using EnvironmentManager.Configuration;

namespace DispenserProvider.Converters;

public static class MapperConfigurationsExtensions
{
    public static IConfigurationProvider WithAwsRegionConverters() => new MapperConfiguration(cfg =>
    {
        DefaultConfigurationExpressions.DefaultConfiguration(cfg);
        cfg.AddProfile<AwsEnvProfile>();
    });
}