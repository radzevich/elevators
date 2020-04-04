using System;
using Elevators.Core.Models;

namespace Elevators.Providers.Currency.Extensions
{
    public static class CurrencyPairExtensions
    {
        private const string Delimiter = "/";
        
        public static string JoinCurrencies(this CurrencyPair currencyPair)
        {
            return string.Join(Delimiter, currencyPair.BaseCurrency, currencyPair.TargetCurrency);
        }
        
        public static (string BaseCurrency, string TargetCurrency) SplitCurrencies(this string currenciesString)
        {
            var currencies = currenciesString.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);

            return (currencies[0], currencies[1]);
        }
    }
}