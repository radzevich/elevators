using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Elevators.Providers.Exmo.Extensions;
using Microsoft.Extensions.Logging;

namespace Elevators.Providers.Exmo
{
    public class ExmoApi : IDisposable
    {
        #region Constants

        private const string HttpClientKey = "exmo";

        #endregion

        #region Fields

        private readonly string _key;
        private readonly string _secret;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public ExmoApi(HttpClient httpClient, ILoggerFactory loggerFactory)
        {
//            _key = key;
            _key = "K-186e91eab9ff2813203850a9a4ff569cdf8e526b";
//            _secret = secret;
            _secret = "S-1a46240920f61360591a00dee984cb091b2e95bb";
            _httpClient = httpClient;
            _logger = loggerFactory.CreateLogger<ExmoApi>();
        }

        #endregion

        public async Task<TResult> SendAsync<TResult>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{nameof(ExmoApi)} call exception");
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<TResult>(responseStream);

            return result;
        }

        public async Task<string> AuthorizedQueryAsync(string apiName, IDictionary<string, string> req)
        {
            req.AddNonceParameter();
            var message = req.ToQueryString();

            var sign = Sign(_secret, message);

            var content = new FormUrlEncodedContent(req);
            content.Headers.Add("Sign", sign);
            content.Headers.Add("Key", _key);

            var response = await _httpClient.PostAsync(apiName, content);

            return await response.Content.ReadAsStringAsync();
        }

        public Task<TResult> AuthorizedQueryAsync<TResult>(string apiName, JsonSerializerOptions jsonSerializerOptions = null)
        {
            return AuthorizedQueryAsync<TResult>(
                apiName,
                new Dictionary<string, string>(),
                jsonSerializerOptions);
        }

        public async Task<TResult> AuthorizedQueryAsync<TResult>(
            string apiName,
            IDictionary<string, string> req,
            JsonSerializerOptions jsonSerializerOptions = null)
        {
            req.AddNonceParameter();
            var message = req.ToQueryString();

            var sign = Sign(_secret, message);

            var content = new FormUrlEncodedContent(req);
            content.Headers.Add("Sign", sign);
            content.Headers.Add("Key", _key);

            var response = await _httpClient.PostAsync(apiName, content);
            await using var responseStream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<TResult>(responseStream, jsonSerializerOptions);

            return result;
        }

        private static string Sign(string key, string message)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var sign = hmac
                .ComputeHash(Encoding.UTF8.GetBytes(message))
                .Aggregate("", (current, t) => current + t.ToString("X2")) // hex
                .ToLowerInvariant();

            return sign;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}