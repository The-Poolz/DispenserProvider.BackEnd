﻿using Nethereum.Util;
using DispenserProvider.Extensions;
using DispenserProvider.DataBase.Models;

namespace DispenserProvider.Services.Handlers.CreateAsset.Models.DatabaseWrappers;

public sealed class BuilderWrapper : BuilderDTO
{
    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Schedule schedule)
    {
        ProviderAddress = schedule.ProviderAddress;
        StartTime = schedule.StartDate;
        FinishTime = schedule.FinishDate.ToUnixTimestamp() == 0 ? null : schedule.FinishDate;
        WeiAmount = user.WeiAmount.MultiplyWeiByRatio(schedule.WeiRatio);
        TransactionDetail = transactionDetail;
    }

    public BuilderWrapper(User user, TransactionDetailWrapper transactionDetail, Refund refund)
    {
        ProviderAddress = refund.DealProvider;
        WeiAmount = user.WeiAmount.MultiplyWeiByRatio(refund.WeiRatio);
        TransactionDetail = transactionDetail;
    }
}