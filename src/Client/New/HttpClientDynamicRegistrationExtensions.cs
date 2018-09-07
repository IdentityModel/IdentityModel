// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public static async Task<RegistrationResponse> RegisterClientAsync(this HttpMessageInvoker client, DynamicClientRegistrationRequest request, CancellationToken cancellationToken = default)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Address)
            {
                Content = new StringContent(JsonConvert.SerializeObject(request.RegistrationRequest), Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Accept.Clear();
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (request.Token.IsPresent())
            {
                httpRequest.SetBearerToken(request.Token);
            }

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new RegistrationResponse(ex);
            }

            if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new RegistrationResponse(content);
            }
            else
            {
                return new RegistrationResponse(response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}