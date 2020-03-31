using System.Collections.Generic;

namespace Elevators.Core.Models
{
    public class Balance
    {
        public Dictionary<string, decimal> Total { get; set; }
        
        public Dictionary<string, decimal> Reserved { get; set; }
    }
}