﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using IdentityModel.Jwk;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// HttpClient extentions for OIDC discovery
    /// </summary>
    public static class HttpClientJsonWebKeySetExtensions
    {
        /// <summary>
        /// Sends a JSON web key set document request
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="address"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<JsonWebKeySetResponse> GetJsonWebKeySetAsync(this HttpMessageInvoker client, string address = null, CancellationToken cancellationToken = default)
        {
            return await client.GetJsonWebKeySetAsync(new JsonWebKeySetRequest
            {
                Address = address
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a JSON web key set document request
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<JsonWebKeySetResponse> GetJsonWebKeySetAsync(this HttpMessageInvoker client, JsonWebKeySetRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            clone.Method = HttpMethod.Get;
            clone.Prepare();

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(clone, cancellationToken).ConfigureAwait(false);

                string responseContent = null;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return await ProtocolResponse.FromHttpResponseAsync<JsonWebKeySetResponse>(response, $"Error connecting to {clone.RequestUri.AbsoluteUri}: {response.ReasonPhrase}").ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                return ProtocolResponse.FromException<JsonWebKeySetResponse>(ex, $"Error connecting to {clone.RequestUri.AbsoluteUri}. {ex.Message}.");
            }

            return await ProtocolResponse.FromHttpResponseAsync<JsonWebKeySetResponse>(response);
        }
    }
}