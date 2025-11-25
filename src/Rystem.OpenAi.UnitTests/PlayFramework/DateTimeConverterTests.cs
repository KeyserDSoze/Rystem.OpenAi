using System.Text.Json;
using Rystem.PlayFramework.Converters;
using Xunit;

namespace Rystem.OpenAi.UnitTests.PlayFramework
{
    /// <summary>
    /// Tests for DateOnly and TimeOnly JSON converters
    /// Ensures LLM-generated dates/times with extra precision are handled correctly
    /// </summary>
    public sealed class DateTimeConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public DateTimeConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new LenientDateOnlyConverter(),
                    new LenientNullableDateOnlyConverter(),
                    new LenientTimeOnlyConverter(),
                    new LenientNullableTimeOnlyConverter()
                }
            };
        }

        #region DateOnly Tests

        [Fact]
        public void DateOnly_CorrectFormat_ShouldDeserialize()
        {
            // Correct format: "YYYY-MM-DD"
            var json = """{"date": "2024-01-15"}""";
            var result = JsonSerializer.Deserialize<DateContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new DateOnly(2024, 1, 15), result.Date);
        }

        [Fact]
        public void DateOnly_WithTime_ShouldDeserialize()
        {
            // LLM often generates: "YYYY-MM-DDTHH:mm:ss"
            var json = """{"date": "2024-01-15T00:00:00"}""";
            var result = JsonSerializer.Deserialize<DateContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new DateOnly(2024, 1, 15), result.Date);
        }

        [Fact]
        public void DateOnly_WithTimeAndZ_ShouldDeserialize()
        {
            // LLM often generates ISO 8601: "YYYY-MM-DDTHH:mm:ssZ"
            var json = """{"date": "2024-01-15T00:00:00Z"}""";
            var result = JsonSerializer.Deserialize<DateContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new DateOnly(2024, 1, 15), result.Date);
        }

        [Fact]
        public void DateOnly_WithFullIso8601_ShouldDeserialize()
        {
            // Full ISO 8601 with timezone
            var json = """{"date": "2024-01-15T14:30:00+01:00"}""";
            var result = JsonSerializer.Deserialize<DateContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new DateOnly(2024, 1, 15), result.Date);
        }

        [Fact]
        public void DateOnly_Serialize_ShouldUseCleanFormat()
        {
            var container = new DateContainer { Date = new DateOnly(2024, 1, 15) };
            var json = JsonSerializer.Serialize(container, _options);

            Assert.Contains("\"2024-01-15\"", json);
            Assert.DoesNotContain("T", json); // No time component
        }

        [Fact]
        public void NullableDateOnly_Null_ShouldDeserialize()
        {
            var json = """{"date": null}""";
            var result = JsonSerializer.Deserialize<NullableDateContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Null(result.Date);
        }

        [Fact]
        public void NullableDateOnly_WithValue_ShouldDeserialize()
        {
            var json = """{"date": "2024-01-15T00:00:00Z"}""";
            var result = JsonSerializer.Deserialize<NullableDateContainer>(json, _options);

            Assert.NotNull(result);
            Assert.NotNull(result.Date);
            Assert.Equal(new DateOnly(2024, 1, 15), result.Date.Value);
        }

        #endregion

        #region TimeOnly Tests

        [Fact]
        public void TimeOnly_CorrectFormat_ShouldDeserialize()
        {
            // Correct format: "HH:mm"
            var json = """{"time": "14:30"}""";
            var result = JsonSerializer.Deserialize<TimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new TimeOnly(14, 30), result.Time);
        }

        [Fact]
        public void TimeOnly_WithSeconds_ShouldDeserialize()
        {
            // Format with seconds: "HH:mm:ss"
            var json = """{"time": "14:30:45"}""";
            var result = JsonSerializer.Deserialize<TimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new TimeOnly(14, 30, 45), result.Time);
        }

        [Fact]
        public void TimeOnly_WithMilliseconds_ShouldDeserialize()
        {
            // Format with milliseconds
            var json = """{"time": "14:30:45.123"}""";
            var result = JsonSerializer.Deserialize<TimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new TimeOnly(14, 30, 45, 123), result.Time);
        }

        [Fact]
        public void TimeOnly_FromDateTime_ShouldDeserialize()
        {
            // LLM might generate full datetime
            var json = """{"time": "2024-01-15T14:30:00"}""";
            var result = JsonSerializer.Deserialize<TimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new TimeOnly(14, 30), result.Time);
        }

        [Fact]
        public void TimeOnly_FromTimeSpan_ShouldDeserialize()
        {
            // TimeSpan format
            var json = """{"time": "14:30:00.0000000"}""";
            var result = JsonSerializer.Deserialize<TimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new TimeOnly(14, 30), result.Time);
        }

        [Fact]
        public void TimeOnly_Serialize_ShouldUseHHmmFormat()
        {
            var container = new TimeContainer { Time = new TimeOnly(14, 30, 45) };
            var json = JsonSerializer.Serialize(container, _options);

            Assert.Contains("\"14:30\"", json);
            Assert.DoesNotContain(":45", json); // No seconds in output
        }

        [Fact]
        public void NullableTimeOnly_Null_ShouldDeserialize()
        {
            var json = """{"time": null}""";
            var result = JsonSerializer.Deserialize<NullableTimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Null(result.Time);
        }

        [Fact]
        public void NullableTimeOnly_WithValue_ShouldDeserialize()
        {
            var json = """{"time": "14:30:00"}""";
            var result = JsonSerializer.Deserialize<NullableTimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.NotNull(result.Time);
            Assert.Equal(new TimeOnly(14, 30), result.Time.Value);
        }

        #endregion

        #region Complex Object Tests

        [Fact]
        public void ComplexObject_WithAllTypes_ShouldDeserialize()
        {
            var json = """
            {
              "startDate": "2024-01-15T00:00:00Z",
              "endDate": "2024-01-20",
              "startTime": "09:00:00",
              "endTime": null
            }
            """;

            var result = JsonSerializer.Deserialize<ComplexDateTimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new DateOnly(2024, 1, 15), result.StartDate);
            Assert.Equal(new DateOnly(2024, 1, 20), result.EndDate);
            Assert.Equal(new TimeOnly(9, 0), result.StartTime);
            Assert.Null(result.EndTime);
        }

        [Fact]
        public void RealWorldScenario_LeaveRequest_ShouldDeserialize()
        {
            // Simulates LLM-generated JSON for leave request
            var json = """
            {
              "taskId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
              "startDate": "2024-06-15T00:00:00.000Z",
              "startTime": "09:00:00",
              "endDate": "2024-06-17",
              "endTime": null,
              "requestReason": "Vacation",
              "approvers": ["manager@test.com"]
            }
            """;

            var result = JsonSerializer.Deserialize<MockLeaveRequest>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), result.TaskId);
            Assert.Equal(new DateOnly(2024, 6, 15), result.StartDate);
            Assert.Equal(new TimeOnly(9, 0), result.StartTime);
            Assert.Equal(new DateOnly(2024, 6, 17), result.EndDate);
            Assert.Null(result.EndTime);
            Assert.Equal("Vacation", result.RequestReason);
            Assert.Single(result.Approvers);
        }

        [Theory]
        [InlineData("2024-01-15")]                      // Clean format
        [InlineData("2024-01-15T00:00:00")]             // With time
        [InlineData("2024-01-15T00:00:00Z")]            // With Z
        [InlineData("2024-01-15T00:00:00.000Z")]        // With milliseconds
        [InlineData("2024-01-15T00:00:00+00:00")]       // With timezone
        [InlineData("2024-01-15T14:30:00Z")]            // Different time
        public void DateOnly_VariousFormats_ShouldAllDeserialize(string dateString)
        {
            var json = $$"""{"date": "{{dateString}}"}""";
            var result = JsonSerializer.Deserialize<DateContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new DateOnly(2024, 1, 15), result.Date);
        }

        [Theory]
        [InlineData("14:30")]                           // HH:mm
        [InlineData("14:30:00")]                        // HH:mm:ss
        [InlineData("14:30:00.000")]                    // With milliseconds
        [InlineData("14:30:00.0000000")]                // TimeSpan format
        [InlineData("2024-01-15T14:30:00")]             // From DateTime (local/unspecified)
        public void TimeOnly_VariousFormats_ShouldAllDeserialize(string timeString)
        {
            var json = $$"""{"time": "{{timeString}}"}""";
            var result = JsonSerializer.Deserialize<TimeContainer>(json, _options);

            Assert.NotNull(result);
            Assert.Equal(new TimeOnly(14, 30), result.Time);
        }

        [Fact]
        public void TimeOnly_FromDateTimeUtc_ShouldPreserveUtcTime()
        {
            // When time comes with Z (UTC), preserve the UTC time value without timezone conversion
            var json = """{"time": "2024-01-15T14:30:00Z"}""";
            var result = JsonSerializer.Deserialize<TimeContainer>(json, _options);

            Assert.NotNull(result);
            // Should be 14:30 UTC, not converted to local time
            Assert.Equal(new TimeOnly(14, 30), result.Time);
        }

        #endregion

        #region Error Handling

        [Fact]
        public void DateOnly_InvalidString_ShouldThrow()
        {
            var json = """{"date": "not-a-date"}""";
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<DateContainer>(json, _options));
        }

        [Fact]
        public void TimeOnly_InvalidString_ShouldThrow()
        {
            var json = """{"time": "not-a-time"}""";
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TimeContainer>(json, _options));
        }

        [Fact]
        public void DateOnly_WrongType_ShouldThrow()
        {
            var json = """{"date": 12345}""";
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<DateContainer>(json, _options));
        }

        [Fact]
        public void TimeOnly_WrongType_ShouldThrow()
        {
            var json = """{"time": 12345}""";
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TimeContainer>(json, _options));
        }

        #endregion
    }

    // Test models - must be public for System.Text.Json
    public class DateContainer
    {
        public DateOnly Date { get; set; }
    }

    public class NullableDateContainer
    {
        public DateOnly? Date { get; set; }
    }

    public class TimeContainer
    {
        public TimeOnly Time { get; set; }
    }

    public class NullableTimeContainer
    {
        public TimeOnly? Time { get; set; }
    }

    public class ComplexDateTimeContainer
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }

    public class MockLeaveRequest
    {
        public Guid TaskId { get; set; }
        public DateOnly StartDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? RequestReason { get; set; }
        public List<string> Approvers { get; set; } = new();
    }
}
