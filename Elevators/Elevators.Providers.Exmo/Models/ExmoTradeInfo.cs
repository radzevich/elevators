using System.Text.Json.Serialization;
using Elevators.Providers.Exmo.Utilities;

namespace Elevators.Providers.Exmo.Models
{
    public class ExmoTradeInfo
    {
        [JsonPropertyName("trade_id")]
        public long TradeId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("quantity")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal Quantity { get; set; }

        [JsonPropertyName("price")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal Price { get; set; }

        [JsonPropertyName("amount")]
        [JsonConverter(typeof(DoubleConverterWithStringSupport))]
        public decimal Amount { get; set; }

        [JsonPropertyName("date")]
        public long Date { get; set; }
    }
}