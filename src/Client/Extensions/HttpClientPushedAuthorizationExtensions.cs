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
    /// <param name="client">The client.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public static Task<PushedAuthorizationResponse> PushAuthorizationRequest(this HttpClient client, PushedAuthorizationRequest request, CancellationToken cancellationToken = default)
    {
        var clone = request.Clone();
    
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.ResponseType, request.ResponseType);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.Scope, request.Scope);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.RedirectUri, request.RedirectUri);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.State, request.State);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.Nonce, request.Nonce);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.LoginHint, request.LoginHint);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.AcrValues, request.AcrValues);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.Prompt, request.Prompt);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.ResponseMode, request.ResponseMode);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.CodeChallenge, request.CodeChallenge);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.CodeChallengeMethod, request.CodeChallengeMethod);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.Display, request.Display);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.MaxAge, request.MaxAge.ToString());
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.UiLocales, request.UiLocales);
        AddOptionalIfNotPresent(clone.Parameters, OidcConstants.AuthorizeRequest.IdTokenHint, request.IdTokenHint);

        return PushAuthorizationRequest(client, clone, cancellationToken);
    }


    internal static async Task<PushedAuthorizationResponse> PushAuthorizationRequest(this HttpMessageInvoker client, ProtocolRequest request, CancellationToken cancellationToken = default)
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


    // Differs from AddOptional with allowDuplicates: false in that duplicates are skipped instead of throwing
    private static void AddOptionalIfNotPresent(Parameters clonedParameters, string key, string? value)
    {
        if (!clonedParameters.ContainsKey(key))
        {
            clonedParameters.AddOptional(key, value);
        }
    }
}
