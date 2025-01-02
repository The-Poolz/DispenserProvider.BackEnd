using System.Numerics;
using Nethereum.Util;

namespace DispenserProvider.Extensions;

public static class AmountExtensions
{
    public static string MultiplyWeiByRatio(this string weiAmount, decimal ratio, int precision = 18) =>
        (BigInteger.Parse(weiAmount) * ConvertToWei(ratio, precision) / BigInteger.Pow(10, precision)).ToString();
    private static BigInteger ConvertToWei(decimal ratio, int precision) => new UnitConversion().ToWei(ratio, precision);
}
