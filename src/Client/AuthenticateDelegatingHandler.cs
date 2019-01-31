// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Internal;

namespace IdentityModel.Client
{
    /// <inheritdoc />
    public sealed class AuthenticateDelegatingHandler : DelegatingHandler
    {
        private readonly IAuthenticationCache _authenticationCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateDelegatingHandler"/> class.
        /// </summary>
        /// <param name="authenticationCache">The authentication cache.</param>
        public AuthenticateDelegatingHandler(IAuthenticationCache authenticationCache)
        {
            _authenticationCache = authenticationCache ?? throw new ArgumentNullException(nameof(authenticationCache));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateDelegatingHandler" /> class.
        /// </summary>
        /// <param name="authenticationCache">The authentication cache.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public AuthenticateDelegatingHandler(IAuthenticationCache authenticationCache, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _authenticationCache = authenticationCache ?? throw new ArgumentNullException(nameof(authenticationCache));
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = _authenticationCache.AccessToken;
            if (accessToken.IsMissing())
            {
                if (await _authenticationCache.RotateAsync(accessToken, cancellationToken).ConfigureAwait(false))
                {
                    accessToken = _authenticationCache.AccessToken;
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) {RequestMessage = request};
                }
            }

            request.Headers.Authorization = new AuthenticationHeaderValue(_authenticationCache.TokenType, accessToken);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (await _authenticationCache.RotateAsync(accessToken, cancellationToken).ConfigureAwait(false))
            {
                accessToken = _authenticationCache.AccessToken;
            }
            else
            {
                return response;
            }

            response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

            request.Headers.Authorization = new AuthenticationHeaderValue(_authenticationCache.TokenType, accessToken);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}