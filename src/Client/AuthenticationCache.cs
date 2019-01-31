// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Internal;

namespace IdentityModel.Client
{
    /// <inheritdoc />
    public sealed class AuthenticationCache : IAuthenticationCache, IDisposable
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly IAuthenticationProvider _authenticationProvider;
        private string _accessToken;
        private string _refreshToken;

        /// <summary>
        /// Occurs when the tokens were received successfully
        /// </summary>
        public event EventHandler<TokenReceivedEventArgs> TokenReceived;

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <inheritdoc />
        public string TokenType { get; set; } = OidcConstants.TokenResponse.BearerTokenType;

        /// <inheritdoc />
        public string AccessToken => _accessToken;

        /// <inheritdoc />
        public string RefreshToken => _refreshToken;

        /// /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationCache"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="accessToken">The access token.</param>
        public AuthenticationCache(IAuthenticationProvider authenticationProvider, string refreshToken = null, string accessToken = null)
        {
            _authenticationProvider = authenticationProvider ?? throw new ArgumentNullException(nameof(authenticationProvider));
            _refreshToken = refreshToken;
            _accessToken = accessToken;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _lock?.Dispose();
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var request = _refreshToken.IsMissing()
                    ? _authenticationProvider.RequestTokenAsync(cancellationToken)
                    : _authenticationProvider.RefreshTokenAsync(_refreshToken, cancellationToken);

                var response = await request.ConfigureAwait(false);
                if (response.IsError)
                {
                    return false;
                }

                Interlocked.Exchange(ref _accessToken, response.AccessToken);
                if (!response.RefreshToken.IsMissing())
                {
                    Interlocked.Exchange(ref _refreshToken, response.RefreshToken);
                }

                TokenReceived?.Invoke(this, new TokenReceivedEventArgs(response.AccessToken, response.RefreshToken));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> RotateAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            if (_accessToken == accessToken)
            {
                if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
                {
                    try
                    {
                        if (_accessToken == accessToken)
                        {
                            return await UpdateAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        _lock.Release();
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}