using System.Text.Json.Serialization;
using Elevators.Providers.Exmo.Utilities;

namespace Elevators.Providers.Exmo.Models
{
    public class ExmoCurrencyPairInfo
    {
        [JsonPropertyName("min_quantity")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal MinQuantity { get; set; }

        [JsonPropertyName("max_quantity")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal MaxQuantity { get; set; }

        [JsonPropertyName("min_price")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal MinPrice { get; set; }

        [JsonPropertyName("max_price")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal MaxPrice { get; set; }

        [JsonPropertyName("min_amount")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal MinAmount { get; set; }

        [JsonPropertyName("max_amount")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal MaxAmount { get; set; }
    }
}