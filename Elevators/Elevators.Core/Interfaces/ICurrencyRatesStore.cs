using System.Collections.Generic;
using Elevators.Core.Models;

namespace Elevators.Core.Interfaces
{
    public interface ICurrencyRatesStore
    {
        IEnumerable<CurrencyRateInfo> GetAll();
        Dictionary<string, decimal> GetLast();
        void Store(Dictionary<string, decimal> rates);
    }
}