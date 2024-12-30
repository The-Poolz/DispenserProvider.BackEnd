using System.Numerics;
using DispenserProvider.Services.Handlers.CreateAsset.Models;

namespace DispenserProvider.Extensions;

public static class AmountExtensions
{
    public static string CalculateAmount(this User user, Schedule schedule) => CalculateAmount(user.WeiAmount, schedule.Ratio);

    public static string CalculateAmount(this User user, Refund refund) => CalculateAmount(user.WeiAmount, refund.Ratio);

    public static string CalculateAmount(this string weiAmount, decimal ratio) => CalculateAmount(BigInteger.Parse(weiAmount), ratio);

    public static string CalculateAmount(this BigInteger weiAmount, decimal ratio) => new BigInteger(ratio * (decimal)weiAmount).ToString();
}