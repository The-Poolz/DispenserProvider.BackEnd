﻿using DispenserProvider.DataBase;
using DispenserProvider.Extensions;
using Microsoft.EntityFrameworkCore;
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
                x.UserAddress == request.UserAddress.Address &&
                ((x.WithdrawalDetail.ChainId == request.ChainId && x.WithdrawalDetail.PoolId == request.PoolId) ||
                 (x.RefundDetail != null && x.RefundDetail.ChainId == request.ChainId && x.RefundDetail.PoolId == request.PoolId))
            ) ?? throw $"Asset by provided PoolId={request.PoolId} and ChainId={request.ChainId} for '{request.UserAddress}' user, not found.".ToException(ErrorCode.DISPENSER_NOT_FOUND);
    }
}