using System.Collections.Generic;
using Elevators.Providers.Exmo.Constants;
using Elevators.Providers.Exmo.Messages;

namespace Elevators.Providers.Exmo.Mappers.In
{
    public static class CreateBuyOrderModelMapper
    {
        public static IDictionary<string, string> Map(ExmoCreateBuyOrderMessage source)
        {
            var pair = $"{source.From}_{source.To}";
            
            var request = new Dictionary<string, string>
            {
                ["pair"] = pair,
                ["quantity"] = source.Amount.ToString("F"),
                ["type"] = ExmoOrderTypes.MarketSell
            };

            return request;
        }
    }
}