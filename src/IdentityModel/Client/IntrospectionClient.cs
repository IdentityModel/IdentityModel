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
    public class IntrospectionClient
    {
        private readonly HttpClient _client;
        private readonly string _clientId;

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

            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
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

            var form = new Dictionary<string, string>();
            form.Add("token", request.Token);

            if (!string.IsNullOrWhiteSpace(request.TokenTypeHint))
            {
                form.Add("token_type_hint", request.TokenTypeHint);
            }

            if (!string.IsNullOrWhiteSpace(request.ClientId))
            {
                form.Add("client_id", request.ClientId);
            }
            else if (!string.IsNullOrWhiteSpace(_clientId))
            {
                form.Add("client_id", _clientId);
            }

            if (!string.IsNullOrWhiteSpace(request.ClientSecret))
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
    }
}