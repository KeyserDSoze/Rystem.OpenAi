using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystem.PlayFramework.Converters
{
    /// <summary>
    /// Lenient JsonConverter for DateOnly that accepts both date-only format and ISO 8601 datetime formats
    /// Handles LLM-generated dates that may include time components
    /// </summary>
    public class LenientDateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected string for DateOnly, got {reader.TokenType}");
            }

            var dateString = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateString))
            {
                throw new JsonException("DateOnly string cannot be null or empty");
            }

            // Try DateOnly format first: "2024-01-15"
            if (DateOnly.TryParse(dateString, out var dateOnly))
            {
                return dateOnly;
            }

            // Try DateTime format and extract date: "2024-01-15T00:00:00" or "2024-01-15T00:00:00Z"
            if (DateTime.TryParse(dateString, out var dateTime))
            {
                return DateOnly.FromDateTime(dateTime);
            }

            throw new JsonException($"Unable to convert '{dateString}' to DateOnly");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            // Always write in clean format: "2024-01-15"
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }

    /// <summary>
    /// Lenient JsonConverter for nullable DateOnly
    /// </summary>
    public class LenientNullableDateOnlyConverter : JsonConverter<DateOnly?>
    {
        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected string or null for DateOnly?, got {reader.TokenType}");
            }

            var dateString = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            // Try DateOnly format first
            if (DateOnly.TryParse(dateString, out var dateOnly))
            {
                return dateOnly;
            }

            // Try DateTime format and extract date
            if (DateTime.TryParse(dateString, out var dateTime))
            {
                return DateOnly.FromDateTime(dateTime);
            }

            throw new JsonException($"Unable to convert '{dateString}' to DateOnly?");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

    /// <summary>
    /// Lenient JsonConverter for TimeOnly that accepts various time formats
    /// Handles LLM-generated times that may include extra precision
    /// </summary>
    public class LenientTimeOnlyConverter : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected string for TimeOnly, got {reader.TokenType}");
            }

            var timeString = reader.GetString();
            if (string.IsNullOrWhiteSpace(timeString))
            {
                throw new JsonException("TimeOnly string cannot be null or empty");
            }

            // Try TimeOnly format: "14:30" or "14:30:00"
            if (TimeOnly.TryParse(timeString, out var timeOnly))
            {
                return timeOnly;
            }

            // Try extracting time from DateTime: "2024-01-15T14:30:00" or "2024-01-15T14:30:00Z"
            // IMPORTANT: Use DateTimeStyles.RoundtripKind to preserve original time without timezone conversion
            if (DateTime.TryParse(timeString, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dateTime))
            {
                // For UTC times (Z suffix), use the UTC time directly without conversion
                if (dateTime.Kind == DateTimeKind.Utc)
                {
                    return new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                }
                return TimeOnly.FromDateTime(dateTime);
            }

            // Try TimeSpan format: "14:30:00.0000000"
            if (TimeSpan.TryParse(timeString, out var timeSpan))
            {
                return TimeOnly.FromTimeSpan(timeSpan);
            }

            throw new JsonException($"Unable to convert '{timeString}' to TimeOnly");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            // Write in HH:mm format (without seconds)
            writer.WriteStringValue(value.ToString("HH:mm"));
        }
    }

    /// <summary>
    /// Lenient JsonConverter for nullable TimeOnly
    /// </summary>
    public class LenientNullableTimeOnlyConverter : JsonConverter<TimeOnly?>
    {
        public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected string or null for TimeOnly?, got {reader.TokenType}");
            }

            var timeString = reader.GetString();
            if (string.IsNullOrWhiteSpace(timeString))
            {
                return null;
            }

            // Try TimeOnly format
            if (TimeOnly.TryParse(timeString, out var timeOnly))
            {
                return timeOnly;
            }

            // Try DateTime extraction with RoundtripKind to preserve original time
            if (DateTime.TryParse(timeString, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dateTime))
            {
                // For UTC times, use the UTC time directly without conversion
                if (dateTime.Kind == DateTimeKind.Utc)
                {
                    return new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                }
                return TimeOnly.FromDateTime(dateTime);
            }

            // Try TimeSpan
            if (TimeSpan.TryParse(timeString, out var timeSpan))
            {
                return TimeOnly.FromTimeSpan(timeSpan);
            }

            throw new JsonException($"Unable to convert '{timeString}' to TimeOnly?");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("HH:mm"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
