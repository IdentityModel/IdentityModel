// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityModel.Client;

/// <summary>
/// Extension methods for IAuthorizeRequestParameters
/// </summary>
public static class AuthorizeRequestParametersExtensions
{
    /// <summary>
    /// Copies properties from a request into a Parameters collection. 
    /// </summary>
    /// <param name="request">The request to copy from.</param>
    /// <param name="targetParameters">The parameters to copy into.</param>
    public static Parameters MergeInto(this IAuthorizeRequestParameters request, Parameters targetParameters)
    {
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.Scope, request.Scope);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.RedirectUri, request.RedirectUri);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.State, request.State);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.Nonce, request.Nonce);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.LoginHint, request.LoginHint);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.AcrValues, request.AcrValues);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.Prompt, request.Prompt);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.ResponseMode, request.ResponseMode);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.CodeChallenge, request.CodeChallenge);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.CodeChallengeMethod, request.CodeChallengeMethod);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.Display, request.Display);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.MaxAge, request.MaxAge.ToString());
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.UiLocales, request.UiLocales);
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.IdTokenHint, request.IdTokenHint);
        foreach(var resource in request.Resource ?? [])
        {
            targetParameters.AddOptional(OidcConstants.AuthorizeRequest.Resource, resource, allowDuplicates: true);
        }
        targetParameters.AddOptional(OidcConstants.AuthorizeRequest.DPoPKeyThumbprint, request.DPoPKeyThumbprint);

        return targetParameters;
    }
}