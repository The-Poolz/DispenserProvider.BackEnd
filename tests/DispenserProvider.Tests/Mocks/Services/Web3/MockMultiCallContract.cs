using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Web3.MultiCall;
using DispenserProvider.Services.Web3.MultiCall.Models;

namespace DispenserProvider.Tests.Mocks.Services.Web3;

internal class MockMultiCallContract : IMultiCallContract
{
    private readonly Dictionary<string, (bool withdraw, bool refund)> _statuses;

    internal ICollection<MultiCallRequest>? LastRequests { get; private set; }

    public MockMultiCallContract(params (DispenserDTO dispenser, bool isWithdrawn, bool isRefunded)[] dispensers)
    {
        _statuses = dispensers.ToDictionary(d => d.dispenser.Id, d => (d.isWithdrawn, d.isRefunded));
    }

    public Task<ICollection<MultiCallResponse>> IsTakenBatchAsync(ICollection<MultiCallRequest> requests)
    {
        LastRequests = requests;
        var responses = requests.Select(r => new MultiCallResponse(r.ChainId,
            r.IsTakenRequests.Select(req =>
            {
                var status = _statuses[req.DispenserId];
                var isTaken = req.IsRefund ? status.refund : status.withdraw;
                return new IsTakenResponse(req.DispenserId, req.IsRefund, isTaken);
            }).ToArray()
        )).ToArray();
        return Task.FromResult<ICollection<MultiCallResponse>>(responses);
    }
}