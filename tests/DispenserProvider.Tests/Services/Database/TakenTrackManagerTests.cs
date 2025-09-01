using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DispenserProvider.DataBase.Models;
using DispenserProvider.Services.Database;
using DispenserProvider.Tests.Mocks.DataBase;
using DispenserProvider.Tests.Mocks.Services.Web3;

namespace DispenserProvider.Tests.Services.Database;

public class TakenTrackManagerTests
{
    public class ProcessTakenTracks
    {
        [Fact]
        internal void WhenWithdrawIsTaken_ShouldAddTrack()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var dispenser = dbFactory.Current.Dispenser.First();
            var multiCall = new MockMultiCallContract((dispenser, true, false));
            var manager = new TakenTrackManager(dbFactory, multiCall);

            var processed = manager.ProcessTakenTracks([dispenser]).ToArray();

            processed.Should().ContainSingle(d => d.Id == dispenser.Id);
            dbFactory.Current.TakenTrack.ToArray().Should().ContainSingle(t =>
                t.DispenserId == dispenser.Id &&
                t.IsRefunded == false
            );
        }

        [Fact]
        internal void WhenRefundIsTaken_ShouldAddTrack()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var dispenser = dbFactory.Current.Dispenser.First();
            dispenser.RefundDetail = new TransactionDetailDTO
            {
                UserAddress = MockDispenserContext.TransactionDetail.UserAddress,
                ChainId = 56,
                PoolId = 1
            };
            var multiCall = new MockMultiCallContract((dispenser, false, true));
            var manager = new TakenTrackManager(dbFactory, multiCall);

            var processed = manager.ProcessTakenTracks([dispenser]).ToArray();

            processed.Should().ContainSingle(d => d.Id == dispenser.Id);
            dbFactory.Current.TakenTrack.ToArray().Should().ContainSingle(t =>
                t.DispenserId == dispenser.Id &&
                t.IsRefunded
            );
        }

        [Fact]
        internal void WhenMultipleChains_ShouldGroupRequestsByChain()
        {
            var dbFactory = new MockDbContextFactory(seed: true);
            var dispenser1 = dbFactory.Current.Dispenser.Include(x => x.WithdrawalDetail).First();
            var secondDispenser = new DispenserDTO(MockDispenserContext.TransactionDetail.UserAddress, withdrawChainId: 1, withdrawPoolId: 1)
            {
                CreationLog = new LogDTO
                {
                    Signature = "0x"
                },
                WithdrawalDetail = new TransactionDetailDTO
                {
                    Id = 2,
                    UserAddress = MockDispenserContext.TransactionDetail.UserAddress,
                    ChainId = 1,
                    PoolId = 1
                }
            };
            dbFactory.Current.Dispenser.Add(secondDispenser);
            dbFactory.Current.TransactionDetails.Add(secondDispenser.WithdrawalDetail!);
            dbFactory.Current.SaveChanges();

            var multiCall = new MockMultiCallContract((dispenser1, false, false), (secondDispenser, false, false));
            var manager = new TakenTrackManager(dbFactory, multiCall);

            _ = manager.ProcessTakenTracks([dispenser1, secondDispenser]).ToArray();

            multiCall.LastRequests.Should().NotBeNull();
            multiCall.LastRequests!.Should().HaveCount(2);
            multiCall.LastRequests.Should().ContainSingle(r => r.ChainId == dispenser1.WithdrawalDetail.ChainId && r.IsTakenRequests.Count == 1);
            multiCall.LastRequests.Should().ContainSingle(r => r.ChainId == secondDispenser.WithdrawalDetail!.ChainId && r.IsTakenRequests.Count == 1);
        }
    }
}