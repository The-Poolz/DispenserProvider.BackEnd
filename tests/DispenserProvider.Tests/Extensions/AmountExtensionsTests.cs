using Xunit;
using FluentAssertions;
using DispenserProvider.Extensions;

namespace DispenserProvider.Tests.Extensions;

public class AmountExtensionsTests
{
    public class MultiplyWeiByRatio
    {
        private const string Zero10 = "0000000000";
        private const string Zero18 = "000000000000000000";

        [Theory]
        [InlineData("100", 1.0, "100", "No ratio change; 100 × 1.0 = 100.")]
        [InlineData("100", 1.5, "150", "Ratio of 1.5 should multiply by 1.5.")]
        [InlineData("100", 0.8, "80", "Ratio of 0.8 means 20% reduction.")]
        [InlineData("100", 0.0, "0", "Ratio of 0.0 should yield zero.")]
        [InlineData("1" + Zero18, 0.667, "66700000" + Zero10, "Large value test, verifying the correct number of zeros and truncation.")]
        [InlineData("10" + Zero10 + Zero18, 0.667, "66700000000" + Zero18, "Even larger test, ensuring correct multiplication of bigger BigInteger.")]
        [InlineData("1" + Zero10 + Zero18 + Zero18, 2, "2" + Zero10 + Zero18 + Zero18, "Doubling a large value by ratio of 2.")]
        [InlineData("1" + Zero18, 0.000000000000000001, "1", "Ratio of 1e-18 yields 1 from 1 × 10^18.")]
        public void ShouldReturnCorrectBigIntegerString(string weiAmount, decimal ratio, string expected, string reason)
        {
            var result = weiAmount.MultiplyWeiByRatio(ratio);

            result.Should().Be(expected, because: reason);
        }
    }
}
