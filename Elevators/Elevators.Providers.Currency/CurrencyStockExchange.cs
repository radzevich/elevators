using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Elevators.Core.Interfaces;
using Elevators.Core.Models;
using Elevators.Providers.Currency.Extensions;

namespace Elevators.Providers.Currency
{
    public class CurrencyStockExchange : IStockExchange
    {
        private readonly CurrencyApi _api;

        public CurrencyStockExchange(CurrencyApi api)
        {
            _api = api;
        }
        
        public Task<IEnumerable<CurrencyPair>> GetCurrencyPairs()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CurrencyRateInfo>> GetActualRatesAsync(IEnumerable<CurrencyPair> currencyPairs, DateTime since)
        {
            _api.SendAsync(HttpMethod.Get, "/api/v1/klines", new Dictionary<string, string>
            {
                ["symbol"] = 
            })
        }
        
        public Task<CurrencyRateInfo> GetActualRatesAsync(CurrencyPair currencyPair, DateTime since)
        {
            _api.SendAsync(HttpMethod.Get, "/api/v1/klines", new Dictionary<string, string>
            {
                ["symbol"] = currencyPair.JoinCurrencies(),
                ["interval"] = "1m",
                ["startTime"] = ((DateTimeOffset) since).ToUnixTimeSeconds().ToString()
            });
        }

        public Task<Balance> GetBalance()
        {
            throw new NotImplementedException();
        }

        public Task<CurrencyRateInfo> CreateOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}