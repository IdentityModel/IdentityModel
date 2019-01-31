// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <inheritdoc />
    public sealed class ResourceOwnerPasswordProvider : IAuthenticationProvider, IDisposable
    {
        private readonly TokenClient _tokenClient;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _scope;
        private readonly object _extra;
        private readonly bool _disposeTokenClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceOwnerPasswordProvider"/> class.
        /// </summary>
        /// <param name="tokenEndpoint">The token endpoint.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="extra">Extra parameters.</param>
        public ResourceOwnerPasswordProvider(string tokenEndpoint, string clientId, string clientSecret, string userName, string password, string scope = null, object extra = null)
            : this(new TokenClient(tokenEndpoint, clientId, clientSecret), userName, password, scope, extra, disposeTokenClient: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceOwnerPasswordProvider"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="extra">Extra parameters.</param>
        public ResourceOwnerPasswordProvider(TokenClient client, string userName, string password, string scope = null, object extra = null, bool disposeTokenClient = false)
        {
            _tokenClient = client ?? throw new ArgumentNullException(nameof(client));
            _disposeTokenClient = disposeTokenClient;
            _userName = userName ?? throw new ArgumentNullException(nameof(userName));
            _password = password ?? throw new ArgumentNullException(nameof(password));
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
            return _tokenClient.RequestResourceOwnerPasswordAsync(_userName, _password, _scope, _extra, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return _tokenClient.RequestRefreshTokenAsync(refreshToken, cancellationToken: cancellationToken);
        }
    }
}