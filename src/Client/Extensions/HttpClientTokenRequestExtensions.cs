﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// HttpClient extensions for OAuth token requests
    /// </summary>
    public static class HttpClientTokenRequestExtensions
    {
        /// <summary>
        /// Sends a token request using the client_credentials grant type.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestClientCredentialsTokenAsync(this HttpMessageInvoker client, ClientCredentialsTokenRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.ClientCredentials);
            clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

            return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a token request using the urn:ietf:params:oauth:grant-type:device_code grant type.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestDeviceTokenAsync(this HttpMessageInvoker client, DeviceTokenRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.DeviceCode);
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.DeviceCode, request.DeviceCode);

            return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a token request using the password grant type.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestPasswordTokenAsync(this HttpMessageInvoker client, PasswordTokenRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.UserName, request.UserName);
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.Password, request.Password, allowEmpty: true);
            clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

            return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a token request using the authorization_code grant type.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestAuthorizationCodeTokenAsync(this HttpMessageInvoker client, AuthorizationCodeTokenRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.AuthorizationCode);
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.Code, request.Code);
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.RedirectUri, request.RedirectUri);
            clone.Parameters.AddOptional(OidcConstants.TokenRequest.CodeVerifier, request.CodeVerifier);

            return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a token request using the refresh_token grant type.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestRefreshTokenAsync(this HttpMessageInvoker client, RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken);
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.RefreshToken, request.RefreshToken);
            clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

            return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a token request.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestTokenAsync(this HttpMessageInvoker client, TokenRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            if (!clone.Parameters.ContainsKey(OidcConstants.TokenRequest.GrantType))
            {
                clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, request.GrantType);
            }

            return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a token request.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="address">The address.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public static async Task<TokenResponse> RequestTokenRawAsync(this HttpMessageInvoker client, string address, IDictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            var request = new TokenRequest()
            {
                Address = address,
                Parameters = parameters
            };

            return await client.RequestTokenAsync(request).ConfigureAwait(false);
        }

        internal static async Task<TokenResponse> RequestTokenAsync(this HttpMessageInvoker client, ProtocolRequest request, CancellationToken cancellationToken = default)
        {
            if (!request.Parameters.TryGetValue(OidcConstants.TokenRequest.ClientId, out _))
            {
                if (request.ClientId.IsMissing()) throw new InvalidOperationException("client_id is missing");
            }

            request.Prepare();
            request.Method = HttpMethod.Post;
            
            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ProtocolResponse.FromException<TokenResponse>(ex);
            }

            return await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(response).ConfigureAwait(false);
        }
    }
}