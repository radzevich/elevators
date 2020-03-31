using System.Collections.Generic;
using System.Threading.Tasks;
using Elevators.Core.Models;

namespace Elevators.Core.Interfaces
{
    public interface ICurrencyRatesProvider
    {
        Task<Dictionary<string, ExchangeInfo>> GetActualCurrenciesAsync();
    }
}