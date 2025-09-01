namespace DispenserProvider.Services.Web3.MultiCall.Models;

public record MultiCallRequest(long ChainId, ICollection<IsTakenRequest> IsTakenRequests);