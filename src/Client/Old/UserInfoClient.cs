// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Client for an OpenID Connect userinfo endpoint
    /// </summary>
    [Obsolete("This type will be deprecated or changed in a future version. It is recommended that you switch to the new extension methods for HttpClient. They give you much more control over the HttpClient lifetime and configuration. See the docs here: https://identitymodel.readthedocs.io")]
    public class UserInfoClient : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// The HTTP client
        /// </summary>
        protected readonly HttpClient Client;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint address.</param>
        public UserInfoClient(string endpoint)
            : this(endpoint, new HttpClientHandler())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint address.</param>
        /// <param name="innerHttpMessageHandler">The inner HTTP message handler.</param>
        /// <exception cref="System.ArgumentNullException">
        /// endpoint
        /// or
        /// innerHttpMessageHandler
        /// </exception>
        public UserInfoClient(string endpoint, HttpMessageHandler innerHttpMessageHandler)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (innerHttpMessageHandler == null) throw new ArgumentNullException(nameof(innerHttpMessageHandler));

            Client = new HttpClient(innerHttpMessageHandler)
            {
                BaseAddress = new Uri(endpoint)
            };

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
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
        /// Sends the userinfo request using the HTTP GET method.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        public virtual async Task<UserInfoResponse> GetAsync(string token, CancellationToken cancellationToken = default)
        {
            if (token.IsMissing()) throw new ArgumentNullException(nameof(token));

            var request = new HttpRequestMessage(HttpMethod.Get, "");
            request.SetBearerToken(token);

            HttpResponseMessage response;
            try
            {
                response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new UserInfoResponse(ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                return new UserInfoResponse(response.StatusCode, response.ReasonPhrase);
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new UserInfoResponse(content);
        }

        /// <summary>
        /// Sends the userinfo request using HTTP POST method.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        public virtual async Task<UserInfoResponse> PostAsync(string token, CancellationToken cancellationToken = default)
        {
            if (token.IsMissing()) throw new ArgumentNullException(nameof(token));

            var request = new HttpRequestMessage(HttpMethod.Post, "");
            request.SetBearerToken(token);

            HttpResponseMessage response;
            try
            {
                response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new UserInfoResponse(ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                return new UserInfoResponse(response.StatusCode, response.ReasonPhrase);
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new UserInfoResponse(content);
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