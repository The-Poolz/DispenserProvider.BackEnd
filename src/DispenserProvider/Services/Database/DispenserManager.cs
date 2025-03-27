using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using Net.Utils.ErrorHandler.Extensions;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database.Models;

namespace DispenserProvider.Services.Database;

public class DispenserManager(IDbContextFactory<DispenserContext> dispenserContextFactory) : IDispenserManager
{
    public DispenserDTO GetDispenser(IGetDispenserRequest request)
    {
        using var dispenserContext = dispenserContextFactory.CreateDbContext();
        return dispenserContext.Dispenser
            .Include(x => x.UserSignatures)
            .Include(x => x.WithdrawalDetail)
            .ThenInclude(x => x.Builders)
            .Include(x => x.RefundDetail)
            .ThenInclude(x => x!.Builders)
            .FirstOrDefault(x =>
                x.DeletionLogSignature == null &&
                x.WithdrawalDetail.UserAddress == request.UserAddress.Address &&
                ((x.WithdrawalDetail.ChainId == request.ChainId && x.WithdrawalDetail.PoolId == request.PoolId) ||
                 (x.RefundDetail != null && x.RefundDetail.ChainId == request.ChainId && x.RefundDetail.PoolId == request.PoolId))
            ) ?? throw ErrorCode.DISPENSER_NOT_FOUND.ToException();
    }
}