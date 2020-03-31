using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elevators.Core.Constants;
using Elevators.Core.Interfaces;
using Elevators.Core.Models;
using Elevators.Services.Utilities;
using Microsoft.Extensions.Logging;

namespace Elevators.Services.Trading
{
    public class TradeService : ITradeService, IDisposable
    {
        private const int DelayStartTradeMs = 0;
        private const int DelayRepeatTradeMs = 60 * 60 * 1000;

        private readonly IStockExchange _stockExchange;
        private readonly ICurrencyRatesStore _currencyRatesStore;
        private readonly ILogger<ITradeService> _logger;

        private RepeatableTask _tradeTask;

        public TradeService(
            IStockExchange stockExchange,
            ICurrencyRatesStore currencyRatesStore,
            ILogger<ITradeService> logger)
        {
            _stockExchange = stockExchange;
            _currencyRatesStore = currencyRatesStore;
            _logger = logger;
        }

        public void Start()
        {
//            _tradeTask = new RepeatableTask(Trade, DelayStartTradeMs, DelayRepeatTradeMs);
            Trade().GetAwaiter().GetResult();
        }

        public async Task Trade()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Start trading");

            var balance = await _stockExchange.GetBalance();

            var baseCurrency = GetBaseCurrency(balance);
            if (baseCurrency == null)
            {
                _logger.LogWarning("No money!");
                return;
            }

            _logger.LogInformation($"Balance is {balance.Total[baseCurrency]} {baseCurrency}");

            var currencyPairs = (await _stockExchange.GetCurrencyPairs()).ToArray();
            var usdPairs = FilterUsdPairs(currencyPairs);

            var availablePairs = FilterPairsByCurrency(currencyPairs, baseCurrency);
            _logger.LogInformation("Available pairs are: {@Pairs}", availablePairs);

            var actualRates = (await _stockExchange.GetActualRatesAsync(usdPairs, DateTime.UtcNow.AddSeconds(-60)))
                .ToDictionary(
                    x => x.BaseCurrency == "USD" ? x.TargetCurrency : x.BaseCurrency,
                    x => x.AverageRate);

            _logger.LogInformation("Current rates: {@Rates}", actualRates);

            // Fallback
            actualRates["USD"] = 1;

            var previousRates = _currencyRatesStore.GetLast();
            if (previousRates != null)
            {
                var rateGrowingCoefficients = GetListOfRateGrowingCoefficients(actualRates, previousRates);
                _logger.LogInformation("Rates growing coefficients are: {@Coefficient}", rateGrowingCoefficients);

                var conversionTypes = GetConversionTypes(baseCurrency, actualRates.Keys, currencyPairs);
            }

            _currencyRatesStore.Store(actualRates);

//            var actualRates = await _currenciesRatesProvider.GetActualCurrenciesAsync();
//            var previousRates = _currencyRatesStore.GetLast();
//            
//            if (previousRates == null || previousRates.Count == 0)
//            {
//                // Doesn't make sense to trade without any historical info
////                await _currencyRatesStore.Store(actualRates);
//                return;
//            }
        }

        private static string GetBaseCurrency(Balance balance)
        {
            foreach (var (currency, total) in balance.Total)
            {
                if (total > 0)
                {
                    return currency;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns list of currency pairs which we can buy by specified currency
        /// </summary>
        /// <param name="pairs"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        private static IEnumerable<CurrencyPair> FilterPairsByCurrency(IEnumerable<CurrencyPair> pairs, string currency)
        {
            return pairs.Where(p => p.BaseCurrency == currency || p.TargetCurrency == currency);
        }

        /// <summary>
        /// Returns list of currency pairs which we can buy by USD
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        private static IEnumerable<CurrencyPair> FilterUsdPairs(IEnumerable<CurrencyPair> pairs)
        {
            return FilterPairsByCurrency(pairs, "USD");
        }

        private static Dictionary<string, decimal> GetListOfRateGrowingCoefficients(
            Dictionary<string, decimal> actualRates,
            Dictionary<string, decimal> previousRates)
        {
            return actualRates.ToDictionary(
                rate => rate.Key,
                rate => {
                    var (currency, actualValue) = rate;
                    var previousValue = previousRates[currency];

                    return actualValue != 0 && previousValue != 0 
                        ? actualValue / previousValue
                        : 0;
                });
        }

        private static Dictionary<string, ConversionType> GetConversionTypes(
            string fromCurrency,
            IEnumerable<string> toCurrencies,
            IEnumerable<CurrencyPair> pairs)
        {
            return toCurrencies.ToDictionary(
                x => x,
                x => GetConversionType(fromCurrency, x, pairs));
        }

        private static ConversionType GetConversionType(string fromCurrency, string toCurrency, IEnumerable<CurrencyPair> pairs)
        {
            if (fromCurrency == toCurrency)
            {
                return ConversionType.None;
            }

            if (pairs.Any(p => p.BaseCurrency == fromCurrency && p.TargetCurrency == toCurrency ||
                               p.BaseCurrency == toCurrency && p.TargetCurrency == fromCurrency))
            {
                return ConversionType.OneStepConversion;
            }
            
            return ConversionType.TwoStepConversion;
        }

        private static decimal CalculateResultValue(
            string fromCurrency,
            string toCurrency,
            Dictionary<string, ConversionType> conversionTypes,
            Dictionary<string, decimal> toUsdCoefficients)
        {
            var conversionType = conversionTypes[fromCurrency];
            var coefficient = toUsdCoefficients[fromCurrency];
            
            switch (conversionType)
            {
                case ConversionType.None:
                    return coefficient;
                case ConversionType.OneStepConversion:
                    return coefficient * 0.098m;
                case ConversionType.TwoStepConversion:
                    return CalculateResultValue(fromCurrency, "USD")
            }
        }

        private IEnumerable<CurrencyPair> FilterAvailablePairs(IEnumerable<CurrencyPair> currencyPairInfo, string baseCurrency)
        {
            return currencyPairInfo
                .Where(cp => cp.BaseCurrency == baseCurrency || cp.TargetCurrency == baseCurrency);
        }

        private decimal CalculatePredictedValueAfterExchange(
            ExchangeInfo previousRate,
            ExchangeInfo actualRate,
            decimal amount)
        {
            return 0;
        }

        private static decimal CalculateUpExchangeCoefficient(
            ExchangeInfo previousRate,
            ExchangeInfo actualRate)
        {
            return CalculateUpExchangeCoefficient(
                previousRate.BuyPrice,
                actualRate.BuyPrice);
        }

        private static decimal CalculateUpExchangeCoefficient(
            decimal previousRate,
            decimal actualRate)
        {
            // y3 = 2 * y2 - y1
            return 2 * actualRate - previousRate;
        }

        private decimal CalculatePredictedRate(
            decimal previousRate,
            decimal actualRate,
            decimal amount)
        {
            // y3 = 2 * y2 - y1
            var coefficient = 2 * actualRate - previousRate;
            var result = amount * coefficient;

            return result;
        }

        public void Dispose()
        {
            _tradeTask?.Dispose();
        }
    }
}