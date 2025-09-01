namespace DispenserProvider.Services.Web3.MultiCall.Models;

public record MultiCallResponse(long ChainId, ICollection<IsTakenResponse> IsTakenResponses);