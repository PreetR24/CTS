using System.Text.Json;
using System.Text.Json.Serialization;

namespace CareSchedule.DTOs
{
    public class StringOrObjectConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString() ?? string.Empty;
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                using (var jsonDoc = JsonDocument.ParseValue(ref reader))
                {
                    return jsonDoc.RootElement.GetRawText();
                }
            }
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing string.");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
