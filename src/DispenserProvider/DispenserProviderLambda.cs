using FluentValidation;
using Amazon.Lambda.Core;
using DispenserProvider.Models;
using DispenserProvider.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DispenserProvider;

public class DispenserProviderLambda(IServiceProvider serviceProvider)
{
    public DispenserProviderLambda() : this(DefaultServiceProvider.Build()) { }

    public LambdaResponse Run(LambdaRequest request)
    {
        try
        {
            return new LambdaResponse(serviceProvider
                .GetRequiredService<IHandlerFactory>()
                .Handle(request)
            );
        }
        catch (ValidationException ex)
        {
            return new LambdaResponse(ex);
        }
        catch (Exception ex)
        {
            LambdaLogger.Log(ex.ToString());
            return new LambdaResponse(ex);
        }
    }
}