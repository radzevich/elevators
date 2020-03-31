﻿﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Web;

namespace ExmoApi.Extensions
{
    public static class RequestExtensions
    {
        private static long _nonce;

        static RequestExtensions()
        {
            _nonce = DateTimeOffset.UtcNow.Millisecond;
        }

        /// <summary>
        /// All the requests should also include the obligatory POST parameter ‘nonce’
        /// with incremental numerical value (>0).
        /// The incremental numerical value should never reiterate or decrease
        /// </summary>
        /// <param name="parameters"></param>
        public static void AddNonceParameter(this IDictionary<string, string> parameters)
        {
            var nonce = Interlocked.Increment(ref _nonce);
            parameters.Add("nonce", Convert.ToString(nonce));
        }

        /// <summary>
        /// Transform request parameters into url query string
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ToQueryString(this IDictionary<string, string> parameters)
        {
            var array = parameters.Keys
                .Select(key => $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(parameters[key])}")
                .ToArray();

            return string.Join("&", array);
        }

        public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> request)
        {
            var nameValueCollection = new NameValueCollection(request?.Count ?? 0);
            if (request != null)
            {
                foreach (var (key, value) in request)
                {
                    nameValueCollection.Add(key, value);
                }
            }

            return nameValueCollection;
        }
    }
}