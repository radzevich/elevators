namespace Elevators.Core.Models
{
    public class CurrencyRateInfo
    {
        public string BaseCurrency { get; set; }

        public string TargetCurrency { get; set; }

        public decimal AverageRate { get; set; }
    }
}