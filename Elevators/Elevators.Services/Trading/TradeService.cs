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
        private const decimal Fee = 0.002m;
        
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

//            var baseCurrency = GetBaseCurrency(balance);
            var baseCurrency = "BTC";
            if (baseCurrency == null)
            {
                _logger.LogWarning("No money!");
                return;
            }

            _logger.LogInformation($"Balance is {balance.Total[baseCurrency]} {baseCurrency}");

            var currencyPairs = (await _stockExchange.GetCurrencyPairs()).ToArray();

            var availablePairs = currencyPairs
                .Where(cp => cp.BaseCurrency == baseCurrency || cp.TargetCurrency == baseCurrency)
                .Select(cp => cp.BaseCurrency == baseCurrency ? cp.TargetCurrency : cp.BaseCurrency)
                .ToHashSet();

            var usdPairs = FilterPairsByCurrency(currencyPairs, "USD").ToArray();
            _logger.LogInformation("USD pairs are: {@Pairs}", usdPairs);

            var actualToUsdRates = (await _stockExchange.GetActualRatesAsync(usdPairs, DateTime.UtcNow.AddSeconds(-60)))
                .ToDictionary(
                    x => x.BaseCurrency == "USD" ? x.TargetCurrency : x.BaseCurrency,
                    x => x.BaseCurrency == "USD"
                        ? x.AverageRate != 0
                            ? 1 / x.AverageRate
                            : 0
                        : x.AverageRate);

//            var actualToBaseRates = (await _stockExchange.GetActualRatesAsync(basePairs, DateTime.UtcNow.AddSeconds(-60)))
//                .Where(x => x.AverageRate != 0)
//                .ToDictionary(
//                    x => x.BaseCurrency == baseCurrency ? x.TargetCurrency : x.BaseCurrency,
//                    x => x.BaseCurrency == baseCurrency ? 1 / x.AverageRate : x.AverageRate);

            _logger.LogInformation("Current rates: {@Rates}", actualToUsdRates);

            var previousToUsdRates = _currencyRatesStore.GetLast();
            if (previousToUsdRates != null)
            {
                var predictedToUsdRates = actualToUsdRates.ToDictionary(
                    x => x.Key,
                    x => CalculatePredictedRate(previousToUsdRates[x.Key], x.Value));

                var (recommendedCurrency, conversionType) = GetRecommendedCurrency(availablePairs, actualToUsdRates, predictedToUsdRates, baseCurrency);
                if (recommendedCurrency == baseCurrency)
                {
                    _logger.LogCritical("NO CONVERSION");
                }
                else
                {
                    _logger.LogCritical($"CONVERT FROM '{baseCurrency}' TO {recommendedCurrency}");
                }
                
                _logger.LogInformation("Recommended conversion: {From} - {To}", baseCurrency, recommendedCurrency);
            }
            

            _currencyRatesStore.Store(actualToUsdRates);
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
            
            return ConversionType.ThroughUsdConversion;
        }

        private static decimal CalculatePredictedRate(
            decimal previousRate,
            decimal actualRate)
        {
            // y3 = 2 * y2 - y1
            if (previousRate == 0 || actualRate == 0)
            {
                return 0;
            }
            return 2 * actualRate - previousRate;
        }

        private (string Currency, ConversionType conversionType) GetRecommendedCurrency(
            ICollection<string> availablePairs,
            IReadOnlyDictionary<string, decimal> actualToUsdRates,
            IReadOnlyDictionary<string, decimal> predictedToUsdRates,
            string baseCurrency)
        {
            var recommendedCurrency = (Currency: "USD", Coefficient: 1m, ConversionType: ConversionType.ToUsd);
            
            Dictionary<string, decimal> coefficients = new Dictionary<string, decimal>();

            foreach (var (currency, coefficient, conversionType) in GetGrowingCoefficients(
                availablePairs,
                actualToUsdRates,
                predictedToUsdRates,
                baseCurrency))
            {
                coefficients[currency] = coefficient;
                if (coefficient > recommendedCurrency.Coefficient)
                {
                    recommendedCurrency = (currency, coefficient, conversionType);
                }
            }
            
            _logger.LogInformation("Coefficients are: {@Coefficients}", coefficients);

            return (recommendedCurrency.Currency, recommendedCurrency.ConversionType);
        }

        private static IEnumerable<(string Currency, decimal Coefficient, ConversionType ConversionType)> GetGrowingCoefficients(
            ICollection<string> availablePairs,
            IReadOnlyDictionary<string, decimal> actualToUsdRates,
            IReadOnlyDictionary<string, decimal> predictedToUsdRates,
            string baseCurrency)
        {
            foreach (var (currency, predictedToUsdRate) in predictedToUsdRates)
            {
                ConversionType conversionType;

                var actualToUsdRate = actualToUsdRates[currency];
                var coefficient = actualToUsdRate != 0
                    ? predictedToUsdRate / actualToUsdRate
                    : 0;

                if (currency == baseCurrency)
                {
                    conversionType = ConversionType.None;
                }
                else if (availablePairs.Contains(currency))
                {
                    coefficient *= (1 - Fee);
                    conversionType = ConversionType.OneStepConversion;
                }
                else
                {
                    coefficient *= (1 - Fee) * (1 - Fee);
                    conversionType = ConversionType.ThroughUsdConversion;
                }

                yield return (currency, coefficient, conversionType);
            }
        }

        public void Dispose()
        {
            _tradeTask?.Dispose();
        }
    }
}