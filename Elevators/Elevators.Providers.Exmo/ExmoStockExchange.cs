using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elevators.Core.Interfaces;
using Elevators.Core.Models;
using Elevators.Providers.Exmo.Mappers.Out;
using Elevators.Providers.Exmo.Utilities;

namespace Elevators.Providers.Exmo
{
    public class ExmoStockExchange : IStockExchange
    {
        private readonly ExmoClient _exmoClient;

        public ExmoStockExchange(ExmoClient exmoClient)
        {
            _exmoClient = exmoClient;
        }
        
        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairs()
        {
            var exmoCurrencyPairsInfo = await _exmoClient.GetCurrencyPairsInfo();
            var currencyPairs = ExmoCurrencyPairInfoMapper.Map(exmoCurrencyPairsInfo);

            return currencyPairs;
        }

        public async Task<IEnumerable<CurrencyRateInfo>> GetActualRatesAsync(IEnumerable<CurrencyPair> currencyPairs, DateTime since)
        {
            var unixSince = ((DateTimeOffset) since).ToUnixTimeSeconds();
            var currencyPairCodes = currencyPairs.Select(CurrencyHelper.JoinCurrencyCodes);

            var exmoTradesInfo = await _exmoClient.GetTrades(currencyPairCodes);
            var recentTradeInfo = exmoTradesInfo.ToDictionary(
                x => x.Key,
                x => x.Value.Where(ti => ti.Date >= unixSince));

            var ratesInfo = ExmoTradeInfoMapper.Map(recentTradeInfo);

            return ratesInfo;
        }

        public async Task<Balance> GetBalance()
        {
            var userInfo = await _exmoClient.GetUserInfo();

            var balance = ExmoUserInfoMapper.Map(userInfo);

            return balance;
        }

        public Task<CurrencyRateInfo> CreateOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}