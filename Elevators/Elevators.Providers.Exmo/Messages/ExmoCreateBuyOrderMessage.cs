namespace Elevators.Providers.Exmo.Messages
{
    public class ExmoCreateBuyOrderMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
    }
}