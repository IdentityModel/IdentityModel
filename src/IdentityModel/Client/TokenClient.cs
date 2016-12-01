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
        protected HttpClient Client;
        private bool _disposed;

        public TokenClient(string address)
            : this(address, new HttpClientHandler())
        { }

        public TokenClient(string address, HttpMessageHandler innerHttpMessageHandler)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            Client = new HttpClient(innerHttpMessageHandler);

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            AuthenticationStyle = AuthenticationStyle.Custom;
            Address = address;
        }

        public TokenClient(string address, string clientId, AuthenticationStyle style = AuthenticationStyle.PostValues)
            : this(address, clientId, string.Empty, new HttpClientHandler(), style)
        { }

        public TokenClient(string address, string clientId, string clientSecret, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, clientId, clientSecret, new HttpClientHandler(), style)
        { }
        
        public TokenClient(string address, string clientId, HttpMessageHandler innerHttpMessageHandler)
            : this(address, clientId, string.Empty, innerHttpMessageHandler, AuthenticationStyle.PostValues)
        { }

        public TokenClient(string address, string clientId, string clientSecret, HttpMessageHandler innerHttpMessageHandler, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, innerHttpMessageHandler)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));

            AuthenticationStyle = style;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Address { get; set; }

        public AuthenticationStyle AuthenticationStyle { get; set; }

        public TimeSpan Timeout
        {
            set
            {
                Client.Timeout = value;
            }
        }

        public virtual async Task<TokenResponse> RequestAsync(IDictionary<string, string> form, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response;

            var request = new HttpRequestMessage(HttpMethod.Post, Address);
            request.Content = new FormUrlEncodedContent(form);

            if (AuthenticationStyle == AuthenticationStyle.BasicAuthentication)
            {
                request.Headers.Authorization = new BasicAuthenticationHeaderValue(ClientId, ClientSecret);
            }

            try
            {
                response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new TokenResponse(ex);
            }

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
                Client.Dispose();
            }
        }
    }
}