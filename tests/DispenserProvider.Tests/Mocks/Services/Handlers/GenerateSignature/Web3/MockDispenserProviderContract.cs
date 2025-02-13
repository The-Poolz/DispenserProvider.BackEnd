using Moq;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Web3;

namespace DispenserProvider.Tests.Mocks.Services.Handlers.GenerateSignature.Web3;

public static class MockDispenserProviderContract
{
    public static IDispenserProviderContract Create(DispenserDTO dispenser, bool isWithdrawn = false, bool isRefunded = false)
    {
        var providerContract = new Mock<IDispenserProviderContract>();

        providerContract.Setup(x => x.IsTaken(dispenser.WithdrawalDetail.ChainId, dispenser.WithdrawalDetail.PoolId, dispenser.UserAddress))
            .Returns(isWithdrawn);

        if (dispenser.RefundDetail != null)
        {
            providerContract.Setup(x => x.IsTaken(dispenser.RefundDetail.ChainId, dispenser.RefundDetail.PoolId, dispenser.UserAddress))
                .Returns(isRefunded);
        }

        return providerContract.Object;
    }
}