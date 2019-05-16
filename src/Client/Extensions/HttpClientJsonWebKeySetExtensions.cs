// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
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
        /// Sends a discovery document request
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="address">The endpoint address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<JsonWebKeyResponse> GetJsonWebKeySetAsync(this HttpMessageInvoker client, string address = null, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;

            using (HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, address))
            {
                try
                {
                    response = await client.SendAsync(getRequest, cancellationToken).ConfigureAwait();

                    string responseContent = null;
                    if (response.Content != null)
                    {
                        responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait();
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        return await ProtocolResponse.FromHttpResponseAsync<JsonWebKeyResponse>(response, $"Error connecting to {address}: {response.ReasonPhrase}").ConfigureAwait();
                    }
                }
                catch (Exception ex)
                {
                    return ProtocolResponse.FromException<JsonWebKeyResponse>(ex, $"Error connecting to {address}. {ex.Message}.");
                }

                return await ProtocolResponse.FromHttpResponseAsync<JsonWebKeyResponse>(response);
            }
        }
    }
}