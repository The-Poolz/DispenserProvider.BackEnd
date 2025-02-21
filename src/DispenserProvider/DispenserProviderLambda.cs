using MediatR;
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

    public async Task<LambdaResponse> RunAsync(LambdaRequest request)
    {
        try
        {
            var response = await serviceProvider
                .GetRequiredService<IMediator>()
                .Send(request.GetRequest(serviceProvider));
            return new LambdaResponse(response);

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