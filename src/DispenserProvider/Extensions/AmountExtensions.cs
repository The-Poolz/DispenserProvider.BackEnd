using System.Numerics;

namespace DispenserProvider.Extensions;

public static class AmountExtensions
{
    public static string MultiplyWeiByRatio(this string weiAmount, string ratio, int precision = 18) =>
        (BigInteger.Parse(weiAmount) * BigInteger.Parse(ratio) / BigInteger.Pow(10, precision)).ToString();
}
