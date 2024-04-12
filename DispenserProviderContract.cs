
namespace DispenserProvider
{
    public class DispenserProviderContract(long chainId)
    {
        private readonly long chainId = chainId;

        internal bool isTaken(long tokenId, string userAddress)
        {
            throw new NotImplementedException();
        }
    }
}