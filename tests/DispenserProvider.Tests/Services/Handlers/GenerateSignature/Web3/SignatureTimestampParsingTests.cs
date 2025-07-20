using Xunit;
using FluentAssertions;
using Nethereum.Util;
using System.Numerics;

namespace DispenserProvider.Tests.Services.Handlers.GenerateSignature.Web3;

/// <summary>
/// Tests for timestamp parsing issues that could cause signature validation failures.
/// Addresses potential parsing problems in DateTime to Unix timestamp conversions and vice versa.
/// </summary>
public class SignatureTimestampParsingTests
{
    public class UnixTimestampRoundTripConversion
    {
        [Fact]
        internal void WhenDateTime_ConvertedToUnixAndBack_ShouldPreserveSecondPrecision()
        {
            // Arrange
            var originalDateTime = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);

            // Act - Round trip conversion: DateTime → Unix → DateTime
            var unixTimestamp = originalDateTime.ToUnixTimestamp();
            var roundTripDateTime = DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp).UtcDateTime;

            // Assert
            roundTripDateTime.Should().Be(originalDateTime,
                "Round-trip conversion should preserve DateTime to second precision");
        }

        [Fact]
        internal void WhenDateTimeWithMilliseconds_ConvertedToUnixAndBack_ShouldTruncateMilliseconds()
        {
            // Arrange
            var originalDateTime = new DateTime(2024, 1, 15, 10, 30, 45, 123, DateTimeKind.Utc);
            var expectedDateTime = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc); // Milliseconds truncated

            // Act - Round trip conversion
            var unixTimestamp = originalDateTime.ToUnixTimestamp();
            var roundTripDateTime = DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp).UtcDateTime;

            // Assert
            roundTripDateTime.Should().Be(expectedDateTime,
                "Unix timestamp conversion should truncate milliseconds consistently");
            
            roundTripDateTime.Should().NotBe(originalDateTime,
                "Original DateTime with milliseconds should differ from truncated version");
        }

        [Theory]
        [InlineData("2024-01-01T00:00:00Z")]     // New Year
        [InlineData("2024-12-31T23:59:59Z")]     // End of year
        [InlineData("2024-02-29T12:00:00Z")]     // Leap year
        [InlineData("2024-07-15T14:30:45Z")]     // Random date
        internal void WhenSpecificDateTime_ConvertedToUnixAndBack_ShouldPreserveValue(string dateTimeString)
        {
            // Arrange
            var originalDateTime = DateTime.Parse(dateTimeString).ToUniversalTime();

            // Act
            var unixTimestamp = originalDateTime.ToUnixTimestamp();
            var roundTripDateTime = DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp).UtcDateTime;

            // Assert
            roundTripDateTime.Should().Be(originalDateTime,
                $"DateTime {dateTimeString} should survive round-trip conversion");
        }
    }

    public class TimezoneConsistency
    {
        [Fact]
        internal void WhenDateTimeLocal_ConvertedToUnix_ShouldConsistentlyUseUtc()
        {
            // Arrange
            var localDateTime = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Local);
            var utcDateTime = localDateTime.ToUniversalTime();

            // Act
            var localUnixTimestamp = localDateTime.ToUnixTimestamp();
            var utcUnixTimestamp = utcDateTime.ToUnixTimestamp();

            // Assert
            utcUnixTimestamp.Should().Be(localUnixTimestamp,
                "Unix timestamp conversion should handle timezone consistently");
        }

        [Fact]
        internal void WhenDateTimeUnspecified_ConvertedToUnix_ShouldBeConsistent()
        {
            // Arrange
            var unspecifiedDateTime = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Unspecified);
            var utcDateTime = DateTime.SpecifyKind(unspecifiedDateTime, DateTimeKind.Utc);

            // Act
            var unspecifiedUnixTimestamp = unspecifiedDateTime.ToUnixTimestamp();
            var utcUnixTimestamp = utcDateTime.ToUnixTimestamp();

            // Assert
            // This test documents the behavior - both should produce the same result
            // as ToUnixTimestamp should treat unspecified as UTC
            unspecifiedUnixTimestamp.Should().Be(utcUnixTimestamp,
                "Unspecified DateTimeKind should be treated consistently with UTC");
        }
    }

    public class EdgeCaseTimestamps
    {
        [Fact]
        internal void WhenUnixEpoch_ConvertedToDateTimeAndBack_ShouldBeConsistent()
        {
            // Arrange
            var epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var expectedUnixTimestamp = new BigInteger(0);

            // Act
            var actualUnixTimestamp = epochDateTime.ToUnixTimestamp();
            var roundTripDateTime = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime;

            // Assert
            actualUnixTimestamp.Should().Be(expectedUnixTimestamp,
                "Unix epoch should convert to timestamp 0");
            
            roundTripDateTime.Should().Be(epochDateTime,
                "Unix timestamp 0 should convert back to epoch DateTime");
        }

        [Fact]
        internal void WhenFutureDateTime_ConvertedToUnix_ShouldProduceLargeTimestamp()
        {
            // Arrange
            var futureDateTime = new DateTime(2050, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var unixTimestamp = futureDateTime.ToUnixTimestamp();
            var roundTripDateTime = DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp).UtcDateTime;

            // Assert
            unixTimestamp.Should().BeGreaterThan(new BigInteger(2524608000), // 2050-01-01 in Unix time
                "Future DateTime should produce large Unix timestamp");
            
            roundTripDateTime.Should().Be(futureDateTime,
                "Future DateTime should survive round-trip conversion");
        }

        [Fact]
        internal void WhenSecondBoundaryDateTime_ConvertedToUnix_ShouldBeConsistent()
        {
            // Arrange - Test dates at exact second boundaries
            var testDateTimes = new[]
            {
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
                new DateTime(2024, 1, 1, 0, 0, 59, DateTimeKind.Utc),
                new DateTime(2024, 1, 1, 0, 1, 0, DateTimeKind.Utc)
            };

            foreach (var dateTime in testDateTimes)
            {
                // Act
                var unixTimestamp = dateTime.ToUnixTimestamp();
                var roundTripDateTime = DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp).UtcDateTime;

                // Assert
                roundTripDateTime.Should().Be(dateTime,
                    $"DateTime {dateTime} at second boundary should survive round-trip conversion");
            }
        }
    }

    public class PrecisionAndRounding
    {
        [Theory]
        [InlineData(0)]    // Exact second
        [InlineData(100)]  // 100ms
        [InlineData(500)]  // 500ms - critical threshold from original issue
        [InlineData(999)]  // 999ms
        internal void WhenDateTimeWithMilliseconds_ConvertedToUnix_ShouldTruncateConsistently(int milliseconds)
        {
            // Arrange
            var baseDateTime = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
            var dateTimeWithMs = baseDateTime.AddMilliseconds(milliseconds);
            var expectedTruncated = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);

            // Act
            var unixTimestamp = dateTimeWithMs.ToUnixTimestamp();
            var roundTripDateTime = DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp).UtcDateTime;

            // Assert
            roundTripDateTime.Should().Be(expectedTruncated,
                $"DateTime with {milliseconds}ms should truncate to second precision");
            
            // Verify the Unix timestamp is the same regardless of milliseconds
            var baseUnixTimestamp = baseDateTime.ToUnixTimestamp();
            unixTimestamp.Should().Be(baseUnixTimestamp,
                $"Unix timestamp should be same for {milliseconds}ms as for 0ms");
        }

        [Fact]
        internal void WhenTwoDateTimesDifferByMilliseconds_UnixTimestampsShouldBeSame()
        {
            // Arrange
            var dateTime1 = new DateTime(2024, 1, 15, 10, 30, 45, 0, DateTimeKind.Utc);
            var dateTime2 = new DateTime(2024, 1, 15, 10, 30, 45, 999, DateTimeKind.Utc);

            // Act
            var unixTimestamp1 = dateTime1.ToUnixTimestamp();
            var unixTimestamp2 = dateTime2.ToUnixTimestamp();

            // Assert
            unixTimestamp1.Should().Be(unixTimestamp2,
                "DateTimes within the same second should produce identical Unix timestamps");
        }
    }

    public class BigIntegerConversion
    {
        [Fact]
        internal void WhenUnixTimestamp_ConvertedToBigInteger_ShouldPreserveValue()
        {
            // Arrange
            var dateTime = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
            var expectedUnixSeconds = new DateTimeOffset(dateTime).ToUnixTimeSeconds();

            // Act
            var bigIntegerTimestamp = dateTime.ToUnixTimestamp();
            var longTimestamp = (long)bigIntegerTimestamp;

            // Assert
            longTimestamp.Should().Be(expectedUnixSeconds,
                "BigInteger Unix timestamp should match DateTimeOffset calculation");
        }

        [Fact]
        internal void WhenLargeUnixTimestamp_ConvertedFromBigInteger_ShouldNotOverflow()
        {
            // Arrange
            var farFutureDateTime = new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            // Act
            var bigIntegerTimestamp = farFutureDateTime.ToUnixTimestamp();

            // Assert
            bigIntegerTimestamp.Should().BePositive("Future timestamps should be positive");
            
            // Verify we can convert back without overflow
            var longTimestamp = (long)bigIntegerTimestamp;
            var roundTripDateTime = DateTimeOffset.FromUnixTimeSeconds(longTimestamp).UtcDateTime;
            
            roundTripDateTime.Should().Be(farFutureDateTime,
                "Large Unix timestamps should convert back correctly");
        }
    }

    public class MultipleConversionConsistency
    {
        [Fact]
        internal void WhenSameDateTimeConvertedMultipleTimes_ShouldProduceSameUnixTimestamp()
        {
            // Arrange
            var dateTime = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);

            // Act - Convert multiple times
            var timestamp1 = dateTime.ToUnixTimestamp();
            var timestamp2 = dateTime.ToUnixTimestamp();
            var timestamp3 = dateTime.ToUnixTimestamp();

            // Assert
            timestamp2.Should().Be(timestamp1, "Multiple conversions should be deterministic");
            timestamp3.Should().Be(timestamp1, "Multiple conversions should be deterministic");
        }

        [Fact]
        internal void WhenDateTimeCreatedDifferentWays_UnixTimestampsShouldMatch()
        {
            // Arrange - Create the same DateTime in different ways
            var dateTime1 = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
            var dateTime2 = DateTime.SpecifyKind(new DateTime(2024, 1, 15, 10, 30, 45), DateTimeKind.Utc);
            var dateTime3 = new DateTimeOffset(2024, 1, 15, 10, 30, 45, TimeSpan.Zero).UtcDateTime;

            // Act
            var timestamp1 = dateTime1.ToUnixTimestamp();
            var timestamp2 = dateTime2.ToUnixTimestamp();
            var timestamp3 = dateTime3.ToUnixTimestamp();

            // Assert
            timestamp2.Should().Be(timestamp1, "SpecifyKind DateTime should produce same timestamp");
            timestamp3.Should().Be(timestamp1, "DateTimeOffset.UtcDateTime should produce same timestamp");
        }
    }
}