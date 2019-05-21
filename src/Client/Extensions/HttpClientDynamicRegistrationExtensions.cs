﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// HttpClient extensions for dynamic registration
    /// </summary>
    public static class HttpClientDynamicRegistrationExtensions
    {
        /// <summary>
        /// Send a dynamic registration request.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<DynamicClientRegistrationResponse> RegisterClientAsync(this HttpMessageInvoker client, DynamicClientRegistrationRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            clone.Method = HttpMethod.Post;
            clone.Content = new StringContent(JsonConvert.SerializeObject(request.Document), Encoding.UTF8, "application/json");
            clone.Prepare();

            if (request.Token.IsPresent())
            {
                clone.SetBearerToken(request.Token);
            }

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(clone, cancellationToken).ConfigureAwait();
            }
            catch (Exception ex)
            {
                return ProtocolResponse.FromException<DynamicClientRegistrationResponse>(ex);
            }

            return await ProtocolResponse.FromHttpResponseAsync<DynamicClientRegistrationResponse>(response).ConfigureAwait();
        }
    }
}