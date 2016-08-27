// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;

namespace System.Net.Http
{
    public static class TokenRevocationClientExtensions
    {
        public static Task<TokenRevocationResponse> RevokeAccessTokenAsync(this TokenRevocationClient client, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.RevokeAsync(token, OidcConstants.TokenResponse.AccessToken, cancellationToken);
        }

        public static Task<TokenRevocationResponse> RevokeRefreshTokenAsync(this TokenRevocationClient client, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.RevokeAsync(token, OidcConstants.TokenResponse.RefreshToken, cancellationToken);
        }
    }
}