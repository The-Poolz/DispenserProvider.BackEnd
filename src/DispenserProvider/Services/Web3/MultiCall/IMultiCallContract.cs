using DispenserProvider.Services.Web3.MultiCall.Models;

namespace DispenserProvider.Services.Web3.MultiCall;

public interface IMultiCallContract
{
    public Task<ICollection<MultiCallResponse>> IsTakenBatchAsync(ICollection<MultiCallRequest> requests);
}