using Xunit;
using FluentAssertions;
using DispenserProvider.Extensions;

namespace DispenserProvider.Tests.Extensions;

public class AmountExtensionsTests
{
    public class CalculateAmount
    {
        [Theory]
        [InlineData("100", 1.0, "100")]
        [InlineData("100", 1.5, "150")]
        [InlineData("100", 0.8, "80")]
        [InlineData("100", 0.0, "0")]
        [InlineData("1000000000000000000", 0.667, "667000000000000000")]
        public void WhenCalledOnString_ShouldReturnCorrectBigIntegerString(string weiAmount, decimal ratio, string expected)
        {
            var result = weiAmount.CalculateAmount(ratio);

            result.Should().Be(expected);
        }
    }
}