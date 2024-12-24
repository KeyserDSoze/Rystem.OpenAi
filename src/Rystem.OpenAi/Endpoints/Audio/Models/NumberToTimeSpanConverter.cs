using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Audio
{
    public class NumberToTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return reader.TryGetDouble(out var value)
                ? TimeSpan.FromSeconds(value)
                : TimeSpan.Zero;
        }

        public override void Write(
            Utf8JsonWriter writer,
            TimeSpan value,
            JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.TotalSeconds);
        }
    }
}
