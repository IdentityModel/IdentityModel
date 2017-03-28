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
    /// <summary>
    /// Client for an OAuth 2.0 token revocation endpoint
    /// </summary>
    public class TokenRevocationClient
    {
        /// <summary>
        /// The client
        /// </summary>
        protected HttpClient Client;

        private readonly string _clientId;

        /// <summary>
        /// Gets or sets the authentication style.
        /// </summary>
        /// <value>
        /// The authentication style.
        /// </value>
        public AuthenticationStyle AuthenticationStyle { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRevocationClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <exception cref="System.ArgumentNullException">endpoint</exception>
        public TokenRevocationClient(string endpoint, string clientId = "", string clientSecret = "", HttpMessageHandler innerHttpMessageHandler = null)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpMessageHandler == null) innerHttpMessageHandler = new HttpClientHandler();

            Client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(endpoint)
            };

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
            {
                Client.SetBasicAuthentication(clientId, clientSecret);
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
                Client.Timeout = value;
            }
        }

        /// <summary>
        /// Sends a token revocation request
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// request
        /// or
        /// Token
        /// </exception>
        public virtual async Task<TokenRevocationResponse> RevokeAsync(
            TokenRevocationRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
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

            try
            {
                var response = await Client.PostAsync("", new FormUrlEncodedContent(form), cancellationToken).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new TokenRevocationResponse();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return new TokenRevocationResponse(content);
                }
                else
                {
                    return new TokenRevocationResponse(response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                return new TokenRevocationResponse(ex);
            }
        }
    }
}