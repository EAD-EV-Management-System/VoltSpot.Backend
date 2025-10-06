using Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common.Converters
{
    public class ChargingTypeJsonConverter : JsonConverter<ChargingType>
    {
        public override ChargingType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (Enum.TryParse<ChargingType>(stringValue, true, out var enumValue))
                {
                    return enumValue;
                }
                throw new JsonException($"Invalid ChargingType value: {stringValue}");
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                var numericValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(ChargingType), numericValue))
                {
                    return (ChargingType)numericValue;
                }
                throw new JsonException($"Invalid ChargingType numeric value: {numericValue}");
            }

            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, ChargingType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class NullableChargingTypeJsonConverter : JsonConverter<ChargingType?>
    {
        public override ChargingType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return null;
                }
                
                if (Enum.TryParse<ChargingType>(stringValue, true, out var enumValue))
                {
                    return enumValue;
                }
                throw new JsonException($"Invalid ChargingType value: {stringValue}");
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                var numericValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(ChargingType), numericValue))
                {
                    return (ChargingType)numericValue;
                }
                throw new JsonException($"Invalid ChargingType numeric value: {numericValue}");
            }

            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, ChargingType? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString());
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}