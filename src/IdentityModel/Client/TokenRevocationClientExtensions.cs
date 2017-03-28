// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Extensions for TokenRevocationClient
    /// </summary>
    public static class TokenRevocationClientExtensions
    {
        /// <summary>
        /// Revokes an access token.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenRevocationResponse> RevokeAccessTokenAsync(this TokenRevocationClient client, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.RevokeAsync(new TokenRevocationRequest
            {
                Token = token,
                TokenTypeHint = "access_token"
            }, cancellationToken);
        }

        /// <summary>
        /// Revokes a refresh token.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenRevocationResponse> RevokeRefreshTokenAsync(this TokenRevocationClient client, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.RevokeAsync(new TokenRevocationRequest
            {
                Token = token,
                TokenTypeHint = "refresh_token"
            }, cancellationToken);
        }
    }
}