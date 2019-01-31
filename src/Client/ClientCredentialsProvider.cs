// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <inheritdoc />
    public sealed class ClientCredentialsProvider : IAuthenticationProvider, IDisposable
    {
        private readonly TokenClient _tokenClient;
        private readonly bool _disposeTokenClient;
        private readonly string _scope;
        private readonly object _extra;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCredentialsProvider"/> class.
        /// </summary>
        /// <param name="tokenEndpoint">The token endpoint.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="extra">Extra parameters.</param>
        public ClientCredentialsProvider(string tokenEndpoint, string clientId, string clientSecret, string scope = null, object extra = null)
            : this(new TokenClient(tokenEndpoint, clientId, clientSecret), scope, extra, disposeTokenClient: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCredentialsProvider"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="extra">Extra parameters.</param>
        public ClientCredentialsProvider(TokenClient client, string scope = null, object extra = null, bool disposeTokenClient = false)
        {
            _tokenClient = client ?? throw new ArgumentNullException(nameof(client));
            _disposeTokenClient = disposeTokenClient;
            _scope = scope;
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
            return _tokenClient.RequestClientCredentialsAsync(_scope, _extra, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return _tokenClient.RequestRefreshTokenAsync(refreshToken, cancellationToken: cancellationToken);
        }
    }
}