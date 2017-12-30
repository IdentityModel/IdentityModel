// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Client for the OAuth 2.0 introspection endpoint
    /// </summary>
    public class IntrospectionClient : IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _clientId;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <exception cref="System.ArgumentNullException">endpoint</exception>
        public IntrospectionClient(string endpoint, string clientId = "", string clientSecret = "", HttpMessageHandler innerHttpMessageHandler = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint)) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpMessageHandler == null) innerHttpMessageHandler = new HttpClientHandler();

            _client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(endpoint)
            };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (clientId.IsPresent() && clientSecret.IsPresent())
            {
                _client.SetBasicAuthentication(clientId, clientSecret);
            }
            else if (!string.IsNullOrWhiteSpace(clientId))
            {
                _clientId = clientId;
            }
        }

        /// <summary>
        /// Sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public TimeSpan Timeout
        {
            set
            {
                _client.Timeout = value;
            }
        }

        /// <summary>
        /// Sends the introspection request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// request
        /// or
        /// Token
        /// </exception>
        public async Task<IntrospectionResponse> SendAsync(IntrospectionRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Token)) throw new ArgumentNullException(nameof(request.Token));

            IDictionary<string, string> form;
            if (request.Parameters == null)
            {
                form = new Dictionary<string, string>();
            }
            else
            {
                form = request.Parameters;
            }

            form.Add("token", request.Token);

            if (request.TokenTypeHint.IsPresent())
            {
                form.Add("token_type_hint", request.TokenTypeHint);
            }

            if (request.ClientId.IsPresent())
            {
                form.Add("client_id", request.ClientId);
            }
            else if (_clientId.IsPresent())
            {
                form.Add("client_id", _clientId);
            }

            if (request.ClientSecret.IsPresent())
            {
                form.Add("client_secret", request.ClientSecret);
            }

            HttpResponseMessage response;
            try
            {
                response = await _client.PostAsync("", new FormUrlEncodedContent(form)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new IntrospectionResponse(ex);
            }
            if (response.IsSuccessStatusCode)
            {
                return new IntrospectionResponse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            else
            {
                return new IntrospectionResponse(response.StatusCode, response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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