using System.Linq;
using Elevators.Core.Models;
using Elevators.Providers.Exmo.Responses;

namespace Elevators.Providers.Exmo.Mappers.Out
{
    public static class ExmoUserInfoMapper
    {
        public static Balance Map(ExmoUserInfoResponse model)
        {
            var total = model.Balances
                .ToDictionary(b => b.Key, b => decimal.Parse(b.Value));
            
            var reserved = model.Balances
                .ToDictionary(b => b.Key, b => decimal.Parse(b.Value));

            return new Balance
            {
                Total = total,
                Reserved = reserved
            };
        }
    }
}