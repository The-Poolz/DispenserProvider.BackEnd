namespace DispenserProvider;

public class DispenserProviderDb 
{
    public required ITransactionDetails WithdrawalDetails { get; set; }
    public ITransactionDetails? RefundDetails { get; set; }
    public bool IsRefund { get; set; }
    public required string UserAddress { get; set; }
    public string Signature { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    internal ITransactionDetails Item => IsRefund ? RefundDetails! : WithdrawalDetails;

    // Generate signature for either withdrawal or refund
    public void GenerateSignature(bool isWithdrawal)
    {
        if (DateTime.Now < ValidUntil)
        {
            throw new InvalidOperationException("Cannot generate signature: the signature is still valid.");
        }
        if (DateTime.Now < ValidUntil.AddMinutes(5))
        {
            throw new InvalidOperationException("Cannot generate signature: the signature is cooldown.");
        }
        IsRefund = !isWithdrawal;
        CheckChains();
        // Placeholder for generating a cryptographic signature
        ValidUntil = DateTime.Now.AddMinutes(5);
        ValidFrom = DateTime.Now.AddSeconds(5);
        Signature = Sign(Item);
    }

    private void CheckChains()
    {

        Check(WithdrawalDetails,"The Deal has been Withdrawen");
        if (RefundDetails != null)
            Check(RefundDetails, "The Deal has been Refunded");
    }
    private string Sign(ITransactionDetails data)
    {
        // this is the data needed for the sign
        return $"{data.PoolId}:{ValidUntil}:{UserAddress}:{data.Builders}";
    }

    private void Check(ITransactionDetails transaction, string message)
    {
        bool isTaken = new DispenserProviderContract(transaction.ChainId).isTaken(transaction.PoolId, UserAddress);
        if (isTaken)
        {
            throw new Exception(message);
        }
    }

    // Get the valid signature based on type
    public SignResultObject GetSignature()
    {
        if (DateTime.Now < ValidFrom)
        {
            throw new InvalidOperationException("Cannot retrieve signature: the validity period has not started yet.");
        }
        if (DateTime.Now > ValidUntil)
        {
            throw new InvalidOperationException("Cannot retrieve signature: the validity period has expired.");
        }
        if (string.IsNullOrEmpty(Signature))
        {
            throw new InvalidOperationException("No signature is available.");
        }
        return new SignResultObject(IsRefund, Item.ChainId, Item.PoolId, UserAddress, Item.Builders, Signature, ValidUntil);
    }
}
