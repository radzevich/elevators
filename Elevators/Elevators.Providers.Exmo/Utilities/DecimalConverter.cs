using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elevators.Providers.Exmo.Utilities
{
    public class DoubleConverterWithStringSupport : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.TokenType == JsonTokenType.String
                ? decimal.Parse(reader.GetString())
                : reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}