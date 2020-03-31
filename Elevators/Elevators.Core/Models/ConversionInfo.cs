using Elevators.Core.Constants;

namespace Elevators.Core.Models
{
    public class ConversionInfo
    {
        public string TargetCurrency { get; set; }
        public ConversionType ConversionType { get; set; }
    }
}