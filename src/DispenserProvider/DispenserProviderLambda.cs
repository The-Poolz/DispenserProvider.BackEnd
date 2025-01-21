using Amazon.Lambda.Core;
using DispenserProvider.Models;
using DispenserProvider.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DispenserProvider;

public class DispenserProviderLambda(IServiceProvider serviceProvider)
{
    public DispenserProviderLambda() : this(DefaultServiceProvider.Build()) { }

    public IHandlerResponse Run(LambdaRequest request)
    {
        return serviceProvider.GetRequiredService<IHandlerFactory>().Handle(request);
    }
}