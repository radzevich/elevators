using System.Text.Json.Serialization;

namespace Elevators.Providers.Exmo.Models
{
    public class ExmoExchangeInfo
    {
        [JsonPropertyName("buy_price")]
        public string BuyPrice { get; set; }
        
        [JsonPropertyName("sell_price")]
        public string SellPrice { get; set; }
        
        [JsonPropertyName("last_trade")]
        public string LastTrade { get; set; }
        
        [JsonPropertyName("high")]
        public string High { get; set; }
        
        [JsonPropertyName("low")]
        public string Low { get; set; }
        
        [JsonPropertyName("avg")]
        public string Avg { get; set; }
        
        [JsonPropertyName("updated")]
        public long Updated { get; set; }
    }
}