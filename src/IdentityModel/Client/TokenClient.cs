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
    /// Client for an OpenID Connect/OAuth 2.0 token endpoint
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TokenClient : IDisposable
    {
        /// <summary>
        /// The client
        /// </summary>
        protected HttpClient Client;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public TokenClient(string address)
            : this(address, new HttpClientHandler())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <exception cref="System.ArgumentNullException">
        /// address
        /// or
        /// innerHttpMessageHandler
        /// </exception>
        public TokenClient(string address, HttpMessageHandler innerHttpMessageHandler)
        {
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            Client = new HttpClient(innerHttpMessageHandler);

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            AuthenticationStyle = AuthenticationStyle.Custom;
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="style">The authentication style.</param>
        public TokenClient(string address, string clientId, AuthenticationStyle style = AuthenticationStyle.PostValues)
            : this(address, clientId, string.Empty, new HttpClientHandler(), style)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="style">The authentication style.</param>
        public TokenClient(string address, string clientId, string clientSecret, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, clientId, clientSecret, new HttpClientHandler(), style)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        public TokenClient(string address, string clientId, HttpMessageHandler innerHttpMessageHandler)
            : this(address, clientId, string.Empty, innerHttpMessageHandler, AuthenticationStyle.PostValues)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenClient"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <param name="style">The authentication style.</param>
        /// <exception cref="System.ArgumentNullException">clientId</exception>
        public TokenClient(string address, string clientId, string clientSecret, HttpMessageHandler innerHttpMessageHandler, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
            : this(address, innerHttpMessageHandler)
        {
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException(nameof(clientId));

            AuthenticationStyle = style;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

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
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the authentication style.
        /// </summary>
        /// <value>
        /// The authentication style.
        /// </value>
        public AuthenticationStyle AuthenticationStyle { get; set; }

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
        /// Sends a token request.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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