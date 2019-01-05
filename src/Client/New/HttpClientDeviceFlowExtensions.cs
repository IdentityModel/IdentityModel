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
    /// HttpClient extensions for OIDC userinfo
    /// </summary>
    public static class HttpClientDeviceFlowExtensions
    {
        /// <summary>
        /// Sends a userinfo request.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<DeviceAuthorizationResponse> RequestDeviceAuthorizationAsync(this HttpMessageInvoker client, DeviceAuthorizationRequest request, CancellationToken cancellationToken = default)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Address);
            httpRequest.Headers.Accept.Clear();
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var clone = request.Clone();
            ClientCredentialsHelper.PopulateClientCredentials(clone, httpRequest);

            clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Scope, request.Scope);

            httpRequest.Content = new FormUrlEncodedContent(clone.Parameters);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new DeviceAuthorizationResponse(ex);
            }

            string content = null;
            if (response.Content != null)
            {
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new DeviceAuthorizationResponse(content);
            }
            else
            {
                return new DeviceAuthorizationResponse(response.StatusCode, response.ReasonPhrase, content);
            }
        }
    }
}