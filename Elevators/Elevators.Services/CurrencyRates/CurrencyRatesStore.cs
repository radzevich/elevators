using System;
using System.Collections.Generic;
using Elevators.Core.Interfaces;
using Elevators.Core.Models;

namespace Elevators.Services.CurrencyRates
{
    public class CurrencyRatesStore : ICurrencyRatesStore
    {
        private Dictionary<string, decimal> _lastRates;
        
        public IEnumerable<CurrencyRateInfo> GetAll()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, decimal> GetLast()
        {
            return _lastRates;
        }

        public void Store(Dictionary<string, decimal> rates)
        {
            _lastRates = rates;
        }
    }
}