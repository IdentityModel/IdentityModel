// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public static partial class TokenRevocationClientExtensions
    {
        public static Task<TokenRevocationResponse> RevokeAccessTokenAsync(this TokenRevocationClient client, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.RevokeAsync(new TokenRevocationRequest
            {
                Token = token,
                TokenTypeHint = "access_token"
            }, cancellationToken);
        }

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