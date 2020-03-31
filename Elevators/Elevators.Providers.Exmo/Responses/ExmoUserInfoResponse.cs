using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Elevators.Providers.Exmo.Responses
{
    public class ExmoUserInfoResponse
    {
        [JsonPropertyName("uid")]
        public long UserId { get; set; }

        [JsonPropertyName("server_date")]
        public long ServerDate { get; set; }

        [JsonPropertyName("balances")]
        public Dictionary<string, string> Balances { get; set; }

        [JsonPropertyName("reserved")]
        public Dictionary<string, string> Reserved { get; set; }
    }
}