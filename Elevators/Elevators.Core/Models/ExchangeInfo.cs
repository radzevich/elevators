using System;

namespace Elevators.Core.Models
{
    public class ExchangeInfo
    {
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal Average { get; set; }
        public decimal LastDeal { get; set; }
        public DateTime UpdatedOn { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
    }
}