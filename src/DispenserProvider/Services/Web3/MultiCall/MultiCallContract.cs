using Nethereum.Contracts;
using DispenserProvider.Services.Web3.MultiCall.Models;
using poolz.finance.csharp.contracts.DispenserProvider.ContractDefinition;

namespace DispenserProvider.Services.Web3.MultiCall;

public class MultiCallContract(IChainProvider chainProvider) : IMultiCallContract
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

        var multiCallFunction = new MultiCallFunction
        {
            Calls = request.IsTakenRequests.Select(x => new Models.MultiCall
            {
                To = dispenserProvider,
                Data = new IsTakenFunction
                {
                    ReturnValue1 = x.PoolId,
                    ReturnValue2 = x.Address
                }.GetCallData()
            }).ToList()
        };

        var handler = web3.Eth.GetContractQueryHandler<MultiCallFunction>();
        var response = await handler.QueryAsync<List<byte[]>>(multiCallAddress, multiCallFunction);

        return new MultiCallResponse(request.ChainId, request.IsTakenRequests
            .Select((x, id) =>
            {
                var bytes = response[id];
                var isTaken = false;
                return new IsTakenResponse(x.DispenserId, x.PoolId, x.Address, x.IsRefund, isTaken);
            }).ToArray()
        );
    }
}