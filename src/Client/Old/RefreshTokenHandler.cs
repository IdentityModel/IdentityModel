// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// HTTP message handler that encapsulates token handling and refresh
    /// </summary>
    [Obsolete("Use RefreshTokenDelegatingHandler (that does not create a default " +
              "inner handler) instead. See " +
              "https://github.com/IdentityModel/IdentityModel2/pull/110", false)]
    public class RefreshTokenHandler : DelegatingHandler
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly TokenClient _tokenClient;
        private string _accessToken;
        private string _refreshToken;
        private bool _disposed;

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
        
        /// <summary>
        /// Gets the current access token
        /// </summary>
        public string AccessToken
        {
            get
            {
                if (_lock.Wait(Timeout))
                {
                    try
                    {
                        return _accessToken;
                    }
                    finally
                    {
                        _lock.Release();
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the current refresh token
        /// </summary>
        public string RefreshToken
        {
            get
            {
                if (_lock.Wait(Timeout))
                {
                    try
                    {
                        return _refreshToken;
                    }
                    finally
                    {
                        _lock.Release();
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Occurs when the tokens were refreshed successfully
        /// </summary>
        public event EventHandler<TokenRefreshedEventArgs> TokenRefreshed = delegate { };

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenHandler"/> class.
        /// </summary>
        /// <param name="tokenEndpoint">The token endpoint.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public RefreshTokenHandler(string tokenEndpoint, string clientId, string clientSecret, string refreshToken, string accessToken = null, HttpMessageHandler innerHandler = null)
            : this(new TokenClient(tokenEndpoint, clientId, clientSecret), refreshToken, accessToken, innerHandler)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenHandler"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public RefreshTokenHandler(TokenClient client, string refreshToken, string accessToken = null, HttpMessageHandler innerHandler = null)
        {
            _tokenClient = client;
            _refreshToken = refreshToken;
            _accessToken = accessToken;

            InnerHandler = innerHandler ?? new HttpClientHandler();
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync(cancellationToken);
            if (accessToken.IsMissing())
            {
                if (await RefreshTokensAsync(cancellationToken) == false)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) {RequestMessage = request};
                }
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (await RefreshTokensAsync(cancellationToken) == false)
            {
                return response;
            }

            response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Net.Http.DelegatingHandler" />, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
          if (disposing && !_disposed) {
              _disposed = true;
              _lock.Dispose();
          }

          base.Dispose(disposing);
        }

        private async Task<bool> RefreshTokensAsync(CancellationToken cancellationToken)
        {
            var refreshToken = RefreshToken;
            if (refreshToken.IsMissing())
            {
                return false;
            }

            if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    var response = await _tokenClient.RequestRefreshTokenAsync(refreshToken, cancellationToken: cancellationToken).ConfigureAwait(false);

                    if (!response.IsError)
                    {
                        _accessToken = response.AccessToken;
                        if (!response.RefreshToken.IsMissing())
                        {
                            _refreshToken = response.RefreshToken;
                        }

#pragma warning disable 4014
                        Task.Run(() =>
                        {
                            foreach (EventHandler<TokenRefreshedEventArgs> del in TokenRefreshed.GetInvocationList())
                            {
                                try
                                {
                                    del(this, new TokenRefreshedEventArgs(response.AccessToken, response.RefreshToken, (int)response.ExpiresIn));
                                }
                                catch { }
                            }
                        }).ConfigureAwait(false);
#pragma warning restore 4014

                        return true;
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }

            return false;
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    return _accessToken;
                }
                finally
                {
                    _lock.Release();
                }
            }

            return null;
        }
    }
}