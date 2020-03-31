using System;
using Elevators.Core.Models;

namespace Elevators.Providers.Exmo.Utilities
{
    public static class CurrencyHelper
    {
        public static (string From, string To) SplitCurrencyCodes(string currencyCodePair)
        {
            var currencyCodePairParts = currencyCodePair?.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (currencyCodePairParts == null || currencyCodePairParts.Length != 2)
            {
                throw new ArgumentException($"Invalid currency code pair value provided: {currencyCodePair}");
            }

            return (currencyCodePairParts[0], currencyCodePairParts[1]);
        }

        public static string JoinCurrencyCodes(CurrencyPair currencyPair)
        {
            return $"{currencyPair.BaseCurrency}_{currencyPair.TargetCurrency}";
        }
    }
}