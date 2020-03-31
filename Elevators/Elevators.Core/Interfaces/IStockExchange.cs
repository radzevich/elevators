using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elevators.Core.Models;

namespace Elevators.Core.Interfaces
{
    public interface IStockExchange
    {
        /// <summary>
        /// Returns the list of currency pairs you are able to trade
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CurrencyPair>> GetCurrencyPairs();
        
        /// <summary>
        /// Returns aggregated information about recent deals for specified currency pairs
        /// </summary>
        /// <param name="currencyPairs"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        Task<IEnumerable<CurrencyRateInfo>> GetActualRatesAsync(IEnumerable<CurrencyPair> currencyPairs, DateTime since);

        Task<Balance> GetBalance();

        /// <summary>
        /// Creates buy/sell order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<CurrencyRateInfo> CreateOrder(Order order);
    }
}