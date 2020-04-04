using Elevators.Core.Interfaces;

namespace Elevators.Services.TrendDetector
{
    public class TrendDetectingService
    {
        private readonly IStockExchange _stockExchange;

        public TrendDetectingService(IStockExchange stockExchange)
        {
            _stockExchange = stockExchange;
        }

        public void Tick()
        {
            _stockExchange.GetActualRatesAsync();
        }
    }
}