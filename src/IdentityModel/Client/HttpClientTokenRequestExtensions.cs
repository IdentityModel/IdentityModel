// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public static async Task<TokenResponse> RequestClientCredentialsTokenAsync(this HttpClient client, ClientCredentialsTokenRequest request, CancellationToken cancellationToken = default)
        {
            request.GrantType = OidcConstants.GrantTypes.ClientCredentials;

            request.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

            return await client.RequestTokenAsync(request, cancellationToken);
        }

        /// <summary>
        /// Sends a token request using the password grant type.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestPasswordTokenAsync(this HttpClient client, PasswordTokenRequest request, CancellationToken cancellationToken = default)
        {
            request.GrantType = OidcConstants.GrantTypes.Password;

            request.Parameters.AddRequired(OidcConstants.TokenRequest.UserName, request.UserName);
            request.Parameters.AddRequired(OidcConstants.TokenRequest.Password, request.Password, allowEmpty: true);
            request.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

            return await client.RequestTokenAsync(request, cancellationToken);
        }

        /// <summary>
        /// Sends a token request using the authorization_code grant type.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestAuthorizationCodeTokenAsync(this HttpClient client, AuthorizationCodeTokenRequest request, CancellationToken cancellationToken = default)
        {
            request.GrantType = OidcConstants.GrantTypes.AuthorizationCode;

            request.Parameters.AddRequired(OidcConstants.TokenRequest.Code, request.Code);
            request.Parameters.AddRequired(OidcConstants.TokenRequest.RedirectUri, request.RedirectUri);
            request.Parameters.AddOptional(OidcConstants.TokenRequest.CodeVerifier, request.CodeVerifier);

            return await client.RequestTokenAsync(request, cancellationToken);
        }

        /// <summary>
        /// Sends a token request using the refresh_token grant type.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestRefreshTokenAsync(this HttpClient client, RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            request.GrantType = OidcConstants.GrantTypes.RefreshToken;

            request.Parameters.AddRequired(OidcConstants.TokenRequest.RefreshToken, request.RefreshToken);
            request.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

            return await client.RequestTokenAsync(request, cancellationToken);
        }

        /// <summary>
        /// Sends a token request.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<TokenResponse> RequestTokenAsync(this HttpClient client, TokenRequest request, CancellationToken cancellationToken = default)
        {
            if (!request.Parameters.ContainsKey(OidcConstants.TokenRequest.GrantType))
            {
                request.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, request.GrantType);
            }

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Address);
            httpRequest.Headers.Accept.Clear();
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ClientCredentialsHelper.PopulateClientCredentials(request, httpRequest);
            httpRequest.Content = new FormUrlEncodedContent(request.Parameters);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new TokenResponse(ex);
            }

            string content = null;
            if (response.Content != null)
            {
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new TokenResponse(content);
            }
            else
            {
                return new TokenResponse(response.StatusCode, response.ReasonPhrase, content);
            }
        }
    }
}