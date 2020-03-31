using System;
using System.Collections.Generic;
using Elevators.Core.Models;
using Elevators.Providers.Exmo.Models;
using Elevators.Providers.Exmo.Utilities;

namespace Elevators.Providers.Exmo.Mappers.Out
{
    public static class ExmoExchangeInfoMapper
    {
        public static List<ExchangeInfo> Map(Dictionary<string, ExmoExchangeInfo> source)
        {
            var result = new List<ExchangeInfo>();

            if (source == null)
            {
                return result;
            }

            foreach (var (currencyPairCode, tickerInnerResult) in source)
            {
                var exchangeInfo = new ExchangeInfo();
                
                var exchangeRate = Map(tickerInnerResult, new ExchangeInfo());
                (exchangeRate.BaseCurrency, exchangeRate.TargetCurrency) = CurrencyHelper.SplitCurrencyCodes(currencyPairCode);

                result.Add(exchangeRate);
            }

            return result;
        }

        private static ExchangeInfo Map(ExmoExchangeInfo source, ExchangeInfo destination)
        {
            destination.Average = decimal.Parse(source.Avg);
            destination.High = decimal.Parse(source.High);
            destination.Low = decimal.Parse(source.Low);
            destination.BuyPrice = decimal.Parse(source.BuyPrice);
            destination.SellPrice = decimal.Parse(source.SellPrice);
            destination.LastDeal = decimal.Parse(source.LastTrade);
            destination.UpdatedOn = DateTimeOffset.FromUnixTimeSeconds(source.Updated).DateTime;

            return destination;
        }

        private static (string From, string To) SplitCurrencyCodes(string currencyCodePair)
        {
            var currencyCodePairParts = currencyCodePair?.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (currencyCodePairParts == null || currencyCodePairParts.Length != 2)
            {
                throw new ArgumentException($"Invalid currency code pair value provided: {currencyCodePair}");
            }

            return (currencyCodePairParts[0], currencyCodePairParts[1]);
        }
    }
}