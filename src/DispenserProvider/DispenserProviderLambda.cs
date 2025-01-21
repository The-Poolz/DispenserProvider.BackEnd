using Amazon.Lambda.Core;
using DispenserProvider.MessageTemplate.Models.Eip712;
using DispenserProvider.Models;
using DispenserProvider.Services;
using EnvironmentManager.Static;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.ABI.EIP712;
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
        var signature = new Eip712TypedDataSigner().SignTypedDataV4<EIP712Domain>(
            request.CreateAssetRequest!.Message.Eip712Message.TypedData.ToJson(),
            new EthECKey(pk)
        );
        Console.WriteLine(signature);
        Console.WriteLine(request.CreateAssetRequest!.Message.Eip712Message.TypedData.ToJson());
        request.CreateAssetRequest.Signature = signature;
        return request;
    }
}