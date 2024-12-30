using Xunit;
using FluentAssertions;
using DispenserProvider.Extensions;

namespace DispenserProvider.Tests.Extensions
{
    /// <summary>
    /// Tests for <see cref="AmountExtensions.MultiplyWeiByRatio"/>.
    /// </summary>
    public class MultiplyWeiByRatioTests
    {
        // Constants for easy zero-padding
        private const string Zero18 = "000000000000000000"; // 18 zeros
        private const string Zero10 = "0000000000";         // 10 zeros

        // Each test case is described in an InlineData attribute:
        // 1. the original weiAmount
        // 2. the ratio (decimal)
        // 3. the expected result (string of a BigInteger)
        // 4. an optional explanation
        [Theory(DisplayName = "MultiplyWeiByRatio should produce correct BigInteger results.")]
        [InlineData("100", 1.0, "100",
            "No ratio change; 100 × 1.0 = 100.")]
        [InlineData("100", 1.5, "150",
            "Ratio of 1.5 should multiply by 1.5.")]
        [InlineData("100", 0.8, "80",
            "Ratio of 0.8 means 20% reduction.")]
        [InlineData("100", 0.0, "0",
            "Ratio of 0.0 should yield zero.")]
        [InlineData("1" + Zero18, 0.667, "66700000" + Zero10,
            "Large value test, verifying the correct number of zeros and truncation.")]
        [InlineData("10" + Zero10 + Zero18, 0.667, "66700000000" + Zero18,
            "Even larger test, ensuring correct multiplication of bigger BigInteger.")]
        [InlineData("1" + Zero10 + Zero18 + Zero18, 2, "2" + Zero10 + Zero18 + Zero18,
            "Doubling a large value by ratio of 2.")]
        public void ShouldReturnCorrectBigIntegerString(
            string weiAmount,
            decimal ratio,
            string expected,
            string reason)
        {
            // Act
            var result = weiAmount.MultiplyWeiByRatio(ratio);

            // Assert
            result.Should().Be(expected, because: reason);
        }
    }
}
