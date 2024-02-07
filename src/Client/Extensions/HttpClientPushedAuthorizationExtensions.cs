// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client;

/// <summary>
/// HttpClient extensions for OIDC discovery
/// </summary>
public static class HttpClientPushedAuthorizationExtensions
{
    /// <summary>
    /// Sends a pushed authorization request
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task<PushedAuthorizationResponse> PushAuthorizationAsync(this HttpClient client, PushedAuthorizationRequest request, CancellationToken cancellationToken = default)
    {
        var clone = request.Clone();

        // client id is always required, and will be added by the call to
        // Prepare for other client credential styles  
        if(request.ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader)
        {
            clone.Parameters.AddRequired(OidcConstants.AuthorizeRequest.ClientId, request.ClientId);
        }
        clone.Parameters.AddRequired(OidcConstants.AuthorizeRequest.ResponseType, request.ResponseType);
        
        request.MergeInto(clone.Parameters);

        return PushAuthorizationAsync(client, clone, cancellationToken);
    }

    /// <summary>
    /// Sends a pushed authorization request
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task<PushedAuthorizationResponse> PushAuthorizationAsync(this HttpClient client, PushedAuthorizationRequestWithRequestObject request, CancellationToken cancellationToken = default)
    {
        var clone = request.Clone();

        // client id is always required, and will be added by the call to
        // Prepare for other client credential styles  
        if(request.ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader)
        {
            clone.Parameters.AddRequired(OidcConstants.AuthorizeRequest.ClientId, request.ClientId);
        }

        clone.Parameters.AddRequired(OidcConstants.AuthorizeRequest.Request, request.Request);

        return PushAuthorizationAsync(client, clone, cancellationToken);
    }

    internal static async Task<PushedAuthorizationResponse> PushAuthorizationAsync(this HttpMessageInvoker client, ProtocolRequest request, CancellationToken cancellationToken = default)
    {
        request.Prepare();
        request.Method = HttpMethod.Post;
            
        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request, cancellationToken).ConfigureAwait();
        }
        catch (OperationCanceledException)
		{
            throw;
		}
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<PushedAuthorizationResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<PushedAuthorizationResponse>(response).ConfigureAwait();
    }
}
