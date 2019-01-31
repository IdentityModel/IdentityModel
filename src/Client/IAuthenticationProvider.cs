// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Interface for authentication provider.
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Requests a token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a token response.</returns>
        Task<TokenResponse> RequestTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes a token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a token response.</returns>
        Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}