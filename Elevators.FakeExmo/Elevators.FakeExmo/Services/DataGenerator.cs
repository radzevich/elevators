using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Elevators.Providers.Exmo.Models;

namespace Elevators.FakeExmo.Services
{
    public class DataGenerator
    {
        private readonly Helpers.CsvHelper.TestTicks[] _testTicks;

        private int _currentTickIndex;

        public DataGenerator()
        {
            _testTicks = Helpers.CsvHelper
                .ReadTestTicks("../../Data/rates.csv")
                .ToArray();
        }
        
        public Dictionary<string, ExmoExchangeInfo> GetNext()
        {
            var nextItem = _testTicks[_currentTickIndex++ % _testTicks.Length];
            var exchangeInfo = new Dictionary<string, ExmoExchangeInfo>
            {
                {
                    "USD_BTC",
                    new ExmoExchangeInfo
                    {
                        Updated = (long) nextItem.Date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        BuyPrice = nextItem.BtcOpen.ToString(CultureInfo.InvariantCulture)
                    }
                },
                {
                    "USD_ETH",
                    new ExmoExchangeInfo
                    {
                        Updated = (long) nextItem.Date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        BuyPrice = nextItem.EthOpen.ToString(CultureInfo.InvariantCulture)
                    }
                },
                {
                    "USD_LTC",
                    new ExmoExchangeInfo
                    {
                        Updated = (long) nextItem.Date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        BuyPrice = nextItem.LtcOpen.ToString(CultureInfo.InvariantCulture)
                    }
                },
                {
                    "USD_XRP",
                    new ExmoExchangeInfo
                    {
                        Updated = (long) nextItem.Date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        BuyPrice = nextItem.XrpOpen.ToString(CultureInfo.InvariantCulture)
                    }
                },
                {
                    "USD_USDT",
                    new ExmoExchangeInfo
                    {
                        Updated = (long) nextItem.Date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        BuyPrice = nextItem.UsdtOpen.ToString(CultureInfo.InvariantCulture)
                    }
                }
            };

            return exchangeInfo;
        }
    }
}