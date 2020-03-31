using System.Text.Json.Serialization;

namespace Elevators.Providers.Exmo.Responses
{
    public class ExmoCreateBuyOrderResponse
    {
        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }
        
        [JsonPropertyName("error")]
        public string Error { get; set; }
        
        [JsonPropertyName("result")]
        public bool Result { get; set; }
    }
}