using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Elevators.Core.Constants;
using Elevators.Core.Models;
using Elevators.Providers.Exmo.Mappers.In;
using Elevators.Providers.Exmo.Messages;
using Elevators.Providers.Exmo.Models;
using Elevators.Providers.Exmo.Responses;

namespace Elevators.Providers.Exmo
{
    public class ExmoClient
    {
        private readonly ExmoApi _exmoApi;

        public ExmoClient(ExmoApi exmoApi)
        {
            _exmoApi = exmoApi;
        }

        public async Task<Dictionary<string, ExmoCurrencyPairInfo>> GetCurrencyPairsInfo()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "pair_settings/");

            var exchangeInfo = await _exmoApi.SendAsync<Dictionary<string, ExmoCurrencyPairInfo>>(request);

            return exchangeInfo;
        }

        public async Task<Dictionary<string, ExmoExchangeInfo>> GetActualCurrenciesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "ticker/");

            var exchangeInfo = await _exmoApi.SendAsync<Dictionary<string, ExmoExchangeInfo>>(request);

            return exchangeInfo;
        }

        public async Task<Dictionary<string, ExmoTradeInfo[]>> GetTrades(IEnumerable<string> currencyPairs)
        {
            var requestUri = currencyPairs.Aggregate(
                new StringBuilder("trades/?pair="),
                (builder, currencyPair) => builder.Append($",{currencyPair}"),
                builder => builder.ToString());

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await _exmoApi.SendAsync<Dictionary<string, ExmoTradeInfo[]>>(request);

            return response;
        }

        public async Task<ExmoUserInfoResponse> GetUserInfo()
        {
            var response = await _exmoApi.AuthorizedQueryAsync<ExmoUserInfoResponse>("user_info");

            return response;
        }

        public StockExchangeInfo GetStockExchangeInfo()
        {
            return new StockExchangeInfo
            {
                DealFee = new DealFee
                {
                    Value = (decimal) 0.002,
                    FeeType = FeeTypes.Relative
                }
            };
        }

        public async Task<ExmoCreateBuyOrderResponse> CreateBuyOrderAsync(ExmoCreateBuyOrderMessage message)
        {
            var request = CreateBuyOrderModelMapper.Map(message);

            var result = await _exmoApi.AuthorizedQueryAsync<ExmoCreateBuyOrderResponse>("order_create", request);

            return result;
        }
    }
}