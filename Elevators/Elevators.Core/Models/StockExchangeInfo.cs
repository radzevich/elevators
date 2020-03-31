namespace Elevators.Core.Models
{
    public class StockExchangeInfo
    {
        public string[] Currencies { get; set; }
        public CurrencyPair[] CurrencyPairsInfo { get; set; }
        public DealFee DealFee { get; set; }
    }
}