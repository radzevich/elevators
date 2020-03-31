using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ExmoApi.Extensions;
using static ExmoApi.Constants;

namespace ExmoApi
{
    public class ExmoApi
    {
        #region Constants

        private const string HttpClientKey = "exmo";

        #endregion
        
        #region Fields

        private readonly string _url;
        private readonly string _key;
        private readonly string _secret;
        private readonly IHttpClientFactory _httpClientFactory;

        #endregion

        #region Constructors

        public ExmoApi(IHttpClientFactory httpClientFactory, string secret, string key)
        {
            _key = key;
            _secret = secret;
            _url = DefaultExmoApiPath;
            _httpClientFactory = httpClientFactory;
        }

        public ExmoApi(IHttpClientFactory httpClientFactory, string secret, string key, string url) : this(httpClientFactory, secret, key)
        {
            _url = url;
        }

        #endregion
        
        public async Task<TResult> SendAsync<TResult>(HttpRequestMessage request)
        {
            using var client = _httpClientFactory.CreateClient(HttpClientKey);
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{nameof(ExmoApi)} call exception");
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(responseStream);

            return result;
        }

        public async Task<string> ApiQueryAsync(string apiName, IDictionary<string, string> req)
        {
            using var client = _httpClientFactory.CreateClient(HttpClientKey);
            req.AddNonceParameter();
            var message = req.ToQueryString();

            var sign = Sign(_secret, message);

            var content = new FormUrlEncodedContent(req);
            content.Headers.Add("Sign", sign);
            content.Headers.Add("Key", _key);

            var response = await client.PostAsync(apiName, content);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<TResult> ApiQueryAsync<TResult>(
            string apiName,
            IDictionary<string, string> req,
            JsonSerializerOptions jsonSerializerOptions = null)
        {
            using var client = _httpClientFactory.CreateClient(HttpClientKey);
            req.AddNonceParameter();
            var message = req.ToQueryString();

            var sign = Sign(_secret, message);

            var content = new FormUrlEncodedContent(req);
            content.Headers.Add("Sign", sign);
            content.Headers.Add("Key", _key);

            var response = await client.PostAsync(apiName, content);
            var responseStream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<TResult>(responseStream, jsonSerializerOptions);

            return result;
        }

        public string ApiQuery(string apiName, IDictionary<string, string> req)
        {
            using var wb = new WebClient();
            req.AddNonceParameter();
            var message = req.ToQueryString();

            var sign = Sign(_secret, message);

            wb.Headers.Add("Sign", sign);
            wb.Headers.Add("Key", _key);

            var data = req.ToNameValueCollection();

            var response = wb.UploadValues($"{_url}/{apiName}", HttpMethod.Post.ToString(), data);
            return Encoding.UTF8.GetString(response);
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
    }
}