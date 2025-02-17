namespace DispenserProvider.Services.Validators.AdminRequest.Models;

public class BuildersValidatorRequest(IEnumerable<ChainAddressPair> withdraw, ChainAddressPair? refund = null)
{
    public IEnumerable<ChainAddressPair> Withdraw { get; } = withdraw;
    public ChainAddressPair? Refund { get; } = refund;
}