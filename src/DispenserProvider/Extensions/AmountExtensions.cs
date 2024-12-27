using System.Numerics;
using DispenserProvider.Services.Handlers.CreateAsset.Models;
using Nethereum.Util;

namespace DispenserProvider.Extensions;

public static class AmountExtensions
{
    public static string CalculateAmount(this User user, Schedule schedule) => CalculateAmount(user.WeiAmount, schedule.Ratio);

    public static string CalculateAmount(this User user, Refund refund) => CalculateAmount(user.WeiAmount, refund.Ratio);

    public static string CalculateAmount(this string weiAmount, decimal ratio) => CalculateAmount(BigDecimal.Parse(weiAmount), ratio);

    public static string CalculateAmount(this BigDecimal weiAmount, decimal ratio) => (new BigDecimal(ratio) * weiAmount).ToString();
}