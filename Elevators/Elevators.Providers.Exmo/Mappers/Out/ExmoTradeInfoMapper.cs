using System.Collections.Generic;
using System.Linq;
using Elevators.Core.Models;
using Elevators.Providers.Exmo.Models;
using Elevators.Providers.Exmo.Utilities;

namespace Elevators.Providers.Exmo.Mappers.Out
{
    
    public static class ExmoTradeInfoMapper
    {
        public static IEnumerable<CurrencyRateInfo> Map(IDictionary<string, IEnumerable<ExmoTradeInfo>> source)
        {
            foreach (var (currencyPair, exmoTradeInfo) in source)
            {
                var count = 0;
                var tradeInfo = new CurrencyRateInfo
                {
                    AverageRate = exmoTradeInfo.Aggregate(0m, (sum, ti) =>
                    {
                        count++;
                        return sum + ti.Price;
                    }, sum => count > 0 ? sum / count : 0)
                };
                
                (tradeInfo.BaseCurrency, tradeInfo.TargetCurrency) = CurrencyHelper.SplitCurrencyCodes(currencyPair);

                yield return tradeInfo;
            }
        }
    }
}