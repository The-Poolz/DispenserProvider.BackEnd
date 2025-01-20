using Nethereum.Util;
using System.Numerics;

namespace DispenserProvider.Extensions;

public static class RatioExtensions
{
    public static decimal StringRatioToDecimal(this string ratio) => UnitConversion.Convert.FromWei(BigInteger.Parse(ratio), 18);
}