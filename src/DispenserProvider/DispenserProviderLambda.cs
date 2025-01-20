using Amazon.Lambda.Core;
using DispenserProvider.Models;
using DispenserProvider.Services;
using EnvironmentManager.Static;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DispenserProvider;

public class DispenserProviderLambda(IServiceProvider serviceProvider)
{
    public DispenserProviderLambda() : this(DefaultServiceProvider.Build()) { }

    public IHandlerResponse Run(LambdaRequest request)
    {
        request = SetRealSignature(request);
        return serviceProvider.GetRequiredService<IHandlerFactory>().Handle(request);
    }

    private static LambdaRequest SetRealSignature(LambdaRequest request)
    {
        var pk = EnvManager.GetRequired<string>("PRIVATE_KEY");
        var signature = new Eip712TypedDataSigner().SignTypedDataV4(
            request.CreateAssetRequest!.Message.Eip712Message,
            request.CreateAssetRequest!.Message.Eip712Message.TypedData,
            new EthECKey(pk)
        );
        Console.WriteLine(signature);
        request.CreateAssetRequest.Signature = signature;
        return request;
    }
}