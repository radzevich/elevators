using System.Collections.Generic;
using Elevators.Core.Models;
using Elevators.Providers.Exmo.Models;
using Elevators.Providers.Exmo.Utilities;

namespace Elevators.Providers.Exmo.Mappers.Out
{
    public static class ExmoCurrencyPairInfoMapper
    {
        public static IEnumerable<CurrencyPair> Map(IDictionary<string, ExmoCurrencyPairInfo> source)
        {
            foreach (var (currencyPairCode, currencyPairInfo) in source)
            {
                var currencyPair = new CurrencyPair
                {
                    MinQuantity = currencyPairInfo.MinQuantity,
                    MaxQuantity = currencyPairInfo.MaxQuantity,
                    MinPrice = currencyPairInfo.MinPrice,
                    MaxPrice = currencyPairInfo.MaxPrice,
                    MaxAmount = currencyPairInfo.MaxAmount,
                    MinAmount = currencyPairInfo.MinAmount
                };

                (currencyPair.BaseCurrency, currencyPair.TargetCurrency) = CurrencyHelper.SplitCurrencyCodes(currencyPairCode);

                yield return currencyPair;
            }
        }
    }
}