using Nethereum.Contracts;
using Nethereum.ABI.Decoders;
using DispenserProvider.Services.Resilience;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using DispenserProvider.Services.Web3.MultiCall.Models;
using poolz.finance.csharp.contracts.DispenserProvider.ContractDefinition;

namespace DispenserProvider.Services.Web3.MultiCall;

public class MultiCallContract(IChainProvider chainProvider, IRetryExecutor retry) : IMultiCallContract
{
    public async Task<ICollection<MultiCallResponse>> IsTakenBatchAsync(ICollection<MultiCallRequest> requests)
    {
        var tasks = requests.Select(IsTakenAsync).ToArray();

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToList();
    }

    private async Task<MultiCallResponse> IsTakenAsync(MultiCallRequest request)
    {
        var web3 = chainProvider.Web3(request.ChainId);
        var multiCallAddress = chainProvider.MultiCallContract(request.ChainId);
        var dispenserProvider = chainProvider.DispenserProviderContract(request.ChainId);

        var multiCallFunction = new Aggregate3Function
        {
            Calls = request.IsTakenRequests.Select(x => new Call3
            {
                AllowFailure = false,
                Target = dispenserProvider,
                CallData = new IsTakenFunction
                {
                    ReturnValue1 = x.PoolId,
                    ReturnValue2 = x.Address
                }.GetCallData()
            }).ToList()
        };

        var handler = web3.Eth.GetContractQueryHandler<Aggregate3Function>();
        var response = await retry.ExecuteAsync(_ => handler.QueryAsync<Aggregate3OutputDTO>(multiCallAddress, multiCallFunction));

        return new MultiCallResponse(request.ChainId, request.IsTakenRequests
            .Select((x, id) =>
            {
                var isTaken = new BoolTypeDecoder().Decode(response.ReturnData[id].ReturnData);
                return new IsTakenResponse(x.DispenserId, x.IsRefund, isTaken);
            }).ToArray()
        );
    }
}