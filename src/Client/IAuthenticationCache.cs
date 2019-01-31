// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Interface for authentication cache.
    /// </summary>
    public interface IAuthenticationCache
    {
        /// <summary>
        /// Gets the token type a.k.a authorization schema
        /// </summary>
        string TokenType { get; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        string RefreshToken { get; }

        /// <summary>
        /// Updates tokens.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <see langword="true" /> if success; otherwise, <see langword="false" />.</returns>
        Task<bool> UpdateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rotates tokens if access tokens are equal.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <see langword="true" /> if success; otherwise, <see langword="false" />.</returns>
        Task<bool> RotateAsync(string accessToken, CancellationToken cancellationToken = default);
    }
}