using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Elevators.Providers.Currency
{
    public class CurrencyApi
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CurrencyApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonSerializerOptions = new JsonSerializerOptions();
        }
        
        public async Task<TResult> SendAsync<TResult>(HttpMethod method, string path, Dictionary<string, string> args)
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = method,
                Content = new FormUrlEncodedContent(args),
                RequestUri = new Uri(path)
            });
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(responseStream, _jsonSerializerOptions);

            return result;
        }
    }
}