// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class UserInfoClient
    {
        private readonly HttpClient _client;

        public UserInfoClient(Uri endpoint, string token)
            : this(endpoint, token, new HttpClientHandler())
        { }

        public UserInfoClient(Uri endpoint, string token, HttpMessageHandler innerHttpMessageHandler)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("token");

            if (innerHttpMessageHandler == null)
                throw new ArgumentNullException("innerHttpMessageHandler");

            _client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = endpoint
            };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _client.SetBearerToken(token);
        }

        public TimeSpan Timeout
        {
            set
            {
                _client.Timeout = value;
            }
        }

        public async Task<UserInfoResponse> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _client.GetAsync("", cancellationToken).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new UserInfoResponse(response.StatusCode, response.ReasonPhrase);
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new UserInfoResponse(content);
        }
    }
}
