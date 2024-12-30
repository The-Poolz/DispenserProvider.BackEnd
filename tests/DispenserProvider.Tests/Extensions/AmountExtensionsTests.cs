using Xunit;
using FluentAssertions;
using DispenserProvider.Extensions;

namespace DispenserProvider.Tests.Extensions;

public class AmountExtensionsTests
{
    public class CalculateAmount
    {
        public const string zeroX18 = "000000000000000000"; // 18 zeros
        public const string zeroX10 = "0000000000"; // 10 zeros
        [Theory]
        [InlineData("100", 1.0, "100")]
        [InlineData("100", 1.5, "150")]
        [InlineData("100", 0.8, "80")]
        [InlineData("100", 0.0, "0")]
        [InlineData("1" + zeroX18, 0.667, "66700000" + zeroX10)]
        [InlineData("10" + zeroX10 + zeroX18, 0.667, "66700000000" + zeroX18)]
        public void WhenCalledOnString_ShouldReturnCorrectBigIntegerString(string weiAmount, decimal ratio, string expected)
        {
            var result = weiAmount.MultiplyWeiByRatio(ratio);

            result.Should().Be(expected);
        }
    }
}