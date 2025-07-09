using System.Numerics;

namespace DispenserProvider.Services.TheGraph.Models;

public record DispenserUpdateParams(BigInteger PoolId, string WeiStartAmount);