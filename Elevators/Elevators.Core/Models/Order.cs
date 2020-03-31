namespace Elevators.Core.Models
{
    public class Order
    {
        public string OrderType { get; set; }
        public decimal Amount { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
    }
}