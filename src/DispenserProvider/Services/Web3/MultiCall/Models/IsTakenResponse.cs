namespace DispenserProvider.Services.Web3.MultiCall.Models;

public record IsTakenResponse(string DispenserId, bool IsRefund, bool IsTaken);