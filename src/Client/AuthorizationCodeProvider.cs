// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <inheritdoc />
    public sealed class AuthorizationCodeProvider : IAuthenticationProvider, IDisposable
    {
        private readonly TokenClient _tokenClient;
        private readonly bool _disposeTokenClient;
        private readonly string _code;
        private readonly string _redirectUri;
        private readonly string _codeVerifier;
        private readonly object _extra;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationCodeProvider"/> class.
        /// </summary>
        /// <param name="tokenEndpoint">The token endpoint.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="code">The code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="codeVerifier">The code verifier.</param>
        /// <param name="extra">Extra parameters.</param>
        public AuthorizationCodeProvider(string tokenEndpoint, string clientId, string clientSecret, string code, string redirectUri, string codeVerifier = null, object extra = null)
            : this(new TokenClient(tokenEndpoint, clientId, clientSecret), code, redirectUri, codeVerifier, extra, disposeTokenClient: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationCodeProvider"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="code">The code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="codeVerifier">The code verifier.</param>
        /// <param name="extra">Extra parameters.</param>
        public AuthorizationCodeProvider(TokenClient client, string code, string redirectUri, string codeVerifier = null, object extra = null, bool disposeTokenClient = false)
        {
            _tokenClient = client ?? throw new ArgumentNullException(nameof(client));
            _disposeTokenClient = disposeTokenClient;
            _code = code ?? throw new ArgumentNullException(nameof(code));
            _redirectUri = redirectUri ?? throw new ArgumentNullException(nameof(redirectUri));
            _codeVerifier = codeVerifier;
            _extra = extra;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposeTokenClient)
            {
                _tokenClient?.Dispose();
            }
        }

        /// <inheritdoc />
        public Task<TokenResponse> RequestTokenAsync(CancellationToken cancellationToken = default)
        {
            return _tokenClient.RequestAuthorizationCodeAsync(_code, _redirectUri, _codeVerifier, _extra, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return _tokenClient.RequestRefreshTokenAsync(refreshToken, cancellationToken: cancellationToken);
        }
    }
}