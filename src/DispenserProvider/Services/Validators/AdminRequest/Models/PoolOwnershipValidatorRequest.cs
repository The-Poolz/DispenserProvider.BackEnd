namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class PoolOwnershipValidatorRequest(ChainPoolPair withdraw, ChainPoolPair? refund = null)
{
    public ChainPoolPair Withdraw { get; } = withdraw;
    public ChainPoolPair? Refund { get; } = refund;
}