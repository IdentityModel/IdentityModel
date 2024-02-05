// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System;
using System.Collections.ObjectModel;
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
    /// <param name="client">The client.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task<PushedAuthorizationResponse> PushAuthorizationAsync(this HttpClient client, PushedAuthorizationRequest request, CancellationToken cancellationToken = default)
    {
        var clone = request.Clone();

        // REVIEW - we aren't doing AddRequired for ClientId, because the later call to Prepare handles
        // parameters needed for client authentication. That means we might not always have a client_id 
        // parameter in the body (it could be in the client assertion or in the authorization header).

        // TODO - Test this
        clone.Parameters.AddRequired(OidcConstants.AuthorizeRequest.ResponseType, request.ResponseType);
        
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Scope, request.Scope);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.RedirectUri, request.RedirectUri);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.State, request.State);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Nonce, request.Nonce);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.LoginHint, request.LoginHint);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.AcrValues, request.AcrValues);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Prompt, request.Prompt);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.ResponseMode, request.ResponseMode);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.CodeChallenge, request.CodeChallenge);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.CodeChallengeMethod, request.CodeChallengeMethod);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Display, request.Display);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.MaxAge, request.MaxAge.ToString());
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.UiLocales, request.UiLocales);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.IdTokenHint, request.IdTokenHint);
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Request, request.Request);
        foreach(var resource in request.Resource ?? [])
        {
            clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Resource, resource, allowDuplicates: true);
        }
        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.DPoPKeyThumbprint, request.DPoPKeyThumbprint);


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
