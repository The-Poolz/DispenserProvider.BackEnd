﻿using System.Numerics;
using Nethereum.Util;

namespace DispenserProvider.Extensions;

public static class AmountExtensions
{
    /// <summary>
    /// Multiplies a large wei-based amount (string) by a decimal ratio,
    /// using Nethereum's UnitConversion to handle big-integer math safely.
    /// </summary>
    /// <param name="weiAmount">The original wei amount (string representation of a BigInteger).</param>
    /// <param name="ratio">A decimal ratio (e.g. 0.667m).</param>
    /// <param name="precision">Number of decimal places to treat the ratio with (default 18).</param>
    /// <returns>String representation of the result as a BigInteger.</returns>
    public static string MultiplyWeiByRatio(this string weiAmount, decimal ratio, int precision = 18)
        => ((BigInteger.Parse(weiAmount) * new UnitConversion().ToWei(ratio, precision))
            / BigInteger.Pow(10, precision))
            .ToString();
}
