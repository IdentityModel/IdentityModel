// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class TokenClient : IDisposable
    {
        protected HttpClient _client;

        private bool _disposed;

        public TokenClient(string address)
            : this(address, new HttpClientHandler())
        { }

        public TokenClient(string address, HttpMessageHandler innerHttpMessageHandler)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (innerHttpMessageHandler == null) throw new ArgumentNullException("innerHttpMessageHandler");

            _client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(address)
            };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            AuthenticationStyle = AuthenticationStyle.None;
        }

        public TokenClient(string address, string clientId, string clientSecret, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, clientId, clientSecret, new HttpClientHandler(), style)
        { }

        public TokenClient(string address, string clientId, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, clientId, string.Empty, new HttpClientHandler(), style)
        { }

        public TokenClient(string address, string clientId, HttpMessageHandler innerHttpMessageHandler)
            : this(address, clientId, string.Empty, innerHttpMessageHandler, AuthenticationStyle.PostValues)
        { }

        public TokenClient(string address, string clientId, string clientSecret, HttpMessageHandler innerHttpMessageHandler, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, innerHttpMessageHandler)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId");

            AuthenticationStyle = style;
            ClientId = clientId;
            ClientSecret = clientSecret;

            if (style == AuthenticationStyle.BasicAuthentication)
            {
                _client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
            }
        }

        public AuthenticationStyle AuthenticationStyle { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public TimeSpan Timeout
        {
            set
            {
                _client.Timeout = value;
            }
        }

        public virtual async Task<TokenResponse> RequestAsync(IDictionary<string, string> form, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _client.PostAsync(string.Empty, new FormUrlEncodedContent(form), cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new TokenResponse(content);
            }
            else
            {
                return new TokenResponse(response.StatusCode, response.ReasonPhrase);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _client.Dispose();
            }
        }
    }
}
