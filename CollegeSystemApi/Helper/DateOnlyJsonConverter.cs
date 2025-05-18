using Newtonsoft.Json;
using System.Globalization;

namespace CollegeSystemApi.Helper;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string OutputFormat = "yyyy-MM-dd";

    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        // Always emit a plain date string
        writer.WriteValue(value.ToString(OutputFormat));
    }

    public override DateOnly ReadJson(JsonReader reader,
                                      Type objectType,
                                      DateOnly existingValue,
                                      bool hasExistingValue,
                                      JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            // ISO string (with or without time zone / time part)
            case JsonToken.String when reader.Value is string s:
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture,
                                      DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                                      out var parsedDt))
                {
                    return DateOnly.FromDateTime(parsedDt);
                }
                throw new JsonSerializationException(
                    $"Invalid date string \"{s}\"; expected ISO format like 2025-05-11 or 2025-05-11T21:00:00Z.");

            // Already parsed as a Date or DateTimeOffset
            case JsonToken.Date:
                return reader.Value switch
                {
                    DateTime dt => DateOnly.FromDateTime(dt),
                    DateTimeOffset dto => DateOnly.FromDateTime(dto.UtcDateTime),
                    _ => throw new JsonSerializationException(
                             $"Unsupported date value type {reader.Value?.GetType().FullName}.")
                };

            // Anything else is invalid
            default:
                throw new JsonSerializationException(
                    $"Unexpected token {reader.TokenType} when parsing DateOnly.");
        }
    }

    public override bool CanRead => true;
}
