// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client;

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
        var clone = request.Clone();

        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Scope, request.Scope);
        clone.Method = HttpMethod.Post;
        clone.Prepare();
        
        // make sure to send form encoded body (even if no parameters are in the body)
        // todo: test with real implementation, maybe turn into a more formal feature
        if (clone.Content == null)
        {
            clone.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());
        }

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(clone, cancellationToken).ConfigureAwait();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<DeviceAuthorizationResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<DeviceAuthorizationResponse>(response).ConfigureAwait();
    }
}