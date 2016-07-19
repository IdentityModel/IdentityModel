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
    /// <summary>
    /// HTTP message handler that encapsulates token handling and refresh
    /// </summary>
    public class RefeshTokenHandler : DelegatingHandler
    {
        private static readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(2);

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly TokenClient _tokenClient;

        private string _accessToken;
        private string _refreshToken;
        private bool _disposed;

        /// <summary>
        /// Gets the current access token
        /// </summary>
        public string AccessToken
        {
            get
            {
                if (_lock.Wait(_lockTimeout))
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
                if (_lock.Wait(_lockTimeout))
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

        public RefeshTokenHandler(string tokenEndpoint, string clientId, string clientSecret, string refreshToken, string accessToken = null, HttpMessageHandler innerHandler = null)
            : this(new TokenClient(tokenEndpoint, clientId, clientSecret), refreshToken, accessToken, innerHandler)
        { }

        public RefeshTokenHandler(TokenClient client, string refreshToken, string accessToken = null, HttpMessageHandler innerHandler = null)
        {
            _tokenClient = client;
            _refreshToken = refreshToken;
            _accessToken = accessToken;

            InnerHandler = innerHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = AccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                if (await RefreshTokensAsync(cancellationToken) == false)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (await RefreshTokensAsync(cancellationToken) == false)
            {
                return response;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }

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
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            if (await _lock.WaitAsync(_lockTimeout, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    var response = await _tokenClient.RequestRefreshTokenAsync(refreshToken, cancellationToken: cancellationToken);

                    if (!response.IsError)
                    {
                        _accessToken = response.AccessToken;
                        _refreshToken = response.RefreshToken;

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
    }
}
