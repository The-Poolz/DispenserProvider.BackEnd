using System.Numerics;

namespace DispenserProvider.Extensions;

public static class AmountExtensions
{
    public static string CalculateAmount(this string weiAmount, decimal ratio) => CalculateAmount(BigInteger.Parse(weiAmount), ratio);

    public static string CalculateAmount(this BigInteger weiAmount, decimal ratio) => new BigInteger(ratio * (decimal)weiAmount).ToString();
}