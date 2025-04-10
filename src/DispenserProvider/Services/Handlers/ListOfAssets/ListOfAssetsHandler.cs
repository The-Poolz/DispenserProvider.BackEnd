﻿using MediatR;
using DispenserProvider.DataBase;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.Services.Database;
using DispenserProvider.Extensions.Pagination;
using DispenserProvider.Services.Handlers.ListOfAssets.Models;

namespace DispenserProvider.Services.Handlers.ListOfAssets;

public class ListOfAssetsHandler(IDbContextFactory<DispenserContext> dispenserContextFactory, ITakenTrackManager takenTrackManager) : IRequestHandler<ListOfAssetsRequest, ListOfAssetsResponse>
{
    public Task<ListOfAssetsResponse> Handle(ListOfAssetsRequest request, CancellationToken cancellationToken)
    {
        var dispenserContext = dispenserContextFactory.CreateDbContext();
        var allDispensers = dispenserContext.Dispenser
            .Where(x =>
                x.WithdrawalDetail.UserAddress == request.UserAddress.Address &&
                x.DeletionLogSignature == null &&
                x.TakenTrack == null
            )
            .Include(x => x.CreationLog)
            .Include(x => x.WithdrawalDetail)
            .ThenInclude(x => x.Builders)
            .Include(x => x.RefundDetail)
            .ThenInclude(x => x!.Builders)
            .ToArray();

        var processed = takenTrackManager
            .ProcessTakenTracks(allDispensers)
            .ToArray();

        var exceptDispensers = allDispensers
            .ExceptBy(processed.Select(x => x.Id), x => x.Id)
            .ToArray();

        var assets = exceptDispensers
            .Paginate(request, x => x.OrderByDescending(d => d.CreationLog.CreationTime))
            .Select(x => new Asset(x));

        return Task.FromResult(new ListOfAssetsResponse(exceptDispensers.Length, assets));
    }
}