using Newtonsoft.Json;
using Nethereum.Signer;
using Amazon.Lambda.Core;
using DispenserProvider.Models;
using DispenserProvider.Services;
using EnvironmentManager.Static;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DispenserProvider;

public class DispenserProviderLambda(IServiceProvider serviceProvider)
{
    public DispenserProviderLambda() : this(DefaultServiceProvider.Default) { }

    public IHandlerResponse Run(LambdaRequest request)
    {
        request = SetRealSignature(request);
        return serviceProvider.GetRequiredService<IHandlerFactory>().Handle(request);
    }

    private static LambdaRequest SetRealSignature(LambdaRequest request)
    {
        var message = JsonConvert.SerializeObject(request.CreateAssetRequest!.Message, Formatting.None);
        var pk = EnvManager.GetRequired<string>("PRIVATE_KEY");
        var signature = new EthereumMessageSigner().EncodeUTF8AndSign(message, new EthECKey(pk));
        request.CreateAssetRequest.Signature = signature;
        return request;
    }
}