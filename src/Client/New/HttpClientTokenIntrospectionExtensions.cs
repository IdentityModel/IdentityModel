// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// HttpClient extensions for OAuth token introspection
    /// </summary>
    public static class HttpClientTokenIntrospectionExtensions
    {
        /// <summary>
        /// Sends an OAuth token introspection request.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<IntrospectionResponse> IntrospectTokenAsync(this HttpMessageInvoker client, TokenIntrospectionRequest request, CancellationToken cancellationToken = default)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Address);
            httpRequest.Headers.Accept.Clear();
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var clone = request.Clone();
            ClientCredentialsHelper.PopulateClientCredentials(clone, httpRequest);

            clone.Parameters.AddRequired(OidcConstants.TokenIntrospectionRequest.Token, request.Token);
            clone.Parameters.AddOptional(OidcConstants.TokenIntrospectionRequest.TokenTypeHint, request.TokenTypeHint);

            httpRequest.Content = new FormUrlEncodedContent(clone.Parameters);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new IntrospectionResponse(ex);
            }
            if (response.IsSuccessStatusCode)
            {
                return new IntrospectionResponse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            else
            {
                return new IntrospectionResponse(response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}