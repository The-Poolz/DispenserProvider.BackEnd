namespace DispenserProvider;

public class SignResultObject
    (bool isRefund, long chainId, 
    long tokenId,
    string userAddress, 
 List<IBuilder> builders,
    string signature,
    DateTime validUntil) 
    : ResultObject(chainId, tokenId, userAddress, builders)
{
    public bool IsRefund { get; set; } = isRefund;
    public string Signature { get; private set; } = signature;
    public DateTime ValidUntil { get; private set; } = validUntil;
}
