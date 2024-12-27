using System.Numerics;
using DispenserProvider.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Extensions;

public static class AmountExtensions
{
    public static string CalculateAmount(this User user, Schedule schedule)
    {
        return CalculateAmount(user.WeiAmount, schedule.Ratio);
    }

    public static string CalculateAmount(this User user, Refund refund)
    {
        return CalculateAmount(user.WeiAmount, refund.Ratio);
    }

    public static string CalculateAmount(this string weiAmount, decimal ratio)
    {
        return new BigInteger(ratio * (decimal)BigInteger.Parse(weiAmount)).ToString();
    }
}