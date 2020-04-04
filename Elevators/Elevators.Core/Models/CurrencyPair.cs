namespace Elevators.Core.Models
{
    public class CurrencyPair
    {
        public string BaseCurrency { get; set; }

        public decimal MinQuantity { get; set; }

        public decimal MaxQuantity { get; set; }

        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }

        public string TargetCurrency { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public string Symbol => $"{BaseCurrency}{Delimiter}{TargetCurrency}";
        
        protected virtual string Delimiter { get; set; }
    }
}