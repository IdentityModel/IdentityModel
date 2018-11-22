// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
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
    [Obsolete("This type will be deprecated or changed in a future version. It is recommended that you switch to the new extension methods for HttpClient. They give you much more control over the HttpClient lifetime and configuration. See the docs here: https://identitymodel.readthedocs.io")]
    public class TokenRevocationClient : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// The HTTP client
        /// </summary>
        protected HttpClient Client;

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        protected string ClientId { get; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public string ClientSecret { get; }

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

            if (clientId.IsPresent() && clientSecret.IsPresent())
            {
                Client.SetBasicAuthenticationOAuth(clientId, clientSecret);
            }
            else if (clientId.IsPresent())
            {
                ClientId = clientId;
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
            CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Token.IsMissing()) throw new ArgumentNullException(nameof(request.Token));

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
            
            if (request.ClientId.IsPresent())
            {
                form.Add("client_id", request.ClientId);
            }
            else if (ClientId.IsPresent())
            {
                form.Add("client_id", ClientId);
            }

            form.AddIfPresent("token_type_hint", request.TokenTypeHint);
            form.AddIfPresent("client_secret", request.ClientSecret);

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
                Client.Dispose();
            }
        }
    }
}