using Elevators.Core.Constants;

namespace Elevators.Core.Models
{
    public class DealFee
    {
        public FeeTypes FeeType { get; set; }
        public decimal Value { get; set; }
    }
}