// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Extensions for RequestUrl
/// </summary>
public static class RequestUrlExtensions
{
    /// <summary>
    /// Creates an authorize URL.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns></returns>
    public static string Create(this RequestUrl request, Parameters parameters)
    {
        return request.Create(parameters);
    }

    /// <summary>
    /// Creates an authorize URL.
    /// </summary>
    /// <param name="requestUrl">The instance of the RequestUrl helper class.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="responseType">The response type.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="redirectUri">The redirect URI.</param>
    /// <param name="state">The state.</param>
    /// <param name="nonce">The nonce.</param>
    /// <param name="loginHint">The login hint.</param>
    /// <param name="acrValues">The acr values.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="responseMode">The response mode.</param>
    /// <param name="codeChallenge">The code challenge.</param>
    /// <param name="codeChallengeMethod">The code challenge method.</param>
    /// <param name="display">The display option.</param>
    /// <param name="maxAge">The max age.</param>
    /// <param name="uiLocales">The ui locales.</param>
    /// <param name="idTokenHint">The id_token hint.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="dpopKeyThumbprint">The dpop_jkt.</param>
    /// <param name="extra">Extra parameters.</param>
    /// <returns></returns>
    public static string CreateAuthorizeUrl(this RequestUrl requestUrl,
        string clientId,
        string responseType,
        string? scope = null,
        string? redirectUri = null,
        string? state = null,
        string? nonce = null,
        string? loginHint = null,
        string? acrValues = null,
        string? prompt = null,
        string? responseMode = null,
        string? codeChallenge = null,
        string? codeChallengeMethod = null,
        string? display = null,
        int? maxAge = null,
        string? uiLocales = null,
        string? idTokenHint = null,
        ICollection<string>? resource = null,
        string? dpopKeyThumbprint = null,
        Parameters? extra = null)
    {
        var authorizeRedirect = new AuthorizeRedirectParameters(clientId, responseType)
        {
            Scope = scope,
            RedirectUri = redirectUri,
            State = state,
            Nonce = nonce,
            LoginHint = loginHint,
            AcrValues = acrValues,
            Prompt = prompt,
            ResponseMode = responseMode,
            CodeChallenge = codeChallenge,
            CodeChallengeMethod = codeChallengeMethod,
            Display = display,
            MaxAge = maxAge,
            UiLocales = uiLocales,
            IdTokenHint = idTokenHint,
            DPoPKeyThumbprint = dpopKeyThumbprint
        };
        if(resource != null)
        {
            authorizeRedirect.Resource = resource;
        }

        return CreateAuthorizeUrl(requestUrl, authorizeRedirect, extra);
    }

    /// <summary>
    /// Creates an authorize URL.
    /// </summary>
    /// <param name="requestUrl">The instance of the RequestUrl helper class.</param>
    /// <param name="redirectParameters">The parameters model.</param>
    /// <param name="extra">Extra parameters.</param>
    public static string CreateAuthorizeUrl(this RequestUrl requestUrl, AuthorizeRedirectParameters redirectParameters, Parameters? extra = null)
    {
        var values = new Parameters();

        values.AddRequired(OidcConstants.AuthorizeRequest.ClientId, redirectParameters.ClientId);
        values.AddRequired(OidcConstants.AuthorizeRequest.ResponseType, redirectParameters.ResponseType);

        redirectParameters.MergeInto(values);

        return requestUrl.Create(values.Merge(extra));
    }

    /// <summary>
    /// Creates an authorize URL using a request_uri parameter.
    /// </summary>
    /// <param name="requestUrl">The instance of the RequestUrl helper class.</param>
    /// <param name="redirectParameters">The parameters model.</param>
    /// <param name="extra">Extra parameters.</param>
    public static string CreateAuthorizeUrl(this RequestUrl requestUrl, RequestUriRedirectParameters redirectParameters, Parameters? extra = null)
    {
        var values = new Parameters();

        values.AddRequired(OidcConstants.AuthorizeRequest.ClientId, redirectParameters.ClientId);
        values.AddRequired(OidcConstants.AuthorizeRequest.RequestUri, redirectParameters.RequestUri);

        return requestUrl.Create(values.Merge(extra));
    }

    /// <summary>
    /// Creates an authorize URL using a request parameter.
    /// </summary>
    /// <param name="requestUrl">The instance of the RequestUrl helper class.</param>
    /// <param name="redirectParameters">The parameters model.</param>
    /// <param name="extra">Extra parameters.</param>
    public static string CreateAuthorizeUrl(this RequestUrl requestUrl, RequestObjectRedirectParameters redirectParameters, Parameters? extra = null)
    {
        var values = new Parameters();

        values.AddRequired(OidcConstants.AuthorizeRequest.ClientId, redirectParameters.ClientId);
        values.AddRequired(OidcConstants.AuthorizeRequest.Request, redirectParameters.Request);

        return requestUrl.Create(values.Merge(extra));
    }

    /// <summary>
    /// Creates an end_session URL.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="idTokenHint">The id_token hint.</param>
    /// <param name="postLogoutRedirectUri">The post logout redirect URI.</param>
    /// <param name="state">The state.</param>
    /// <param name="extra">The extra parameters.</param>
    /// <returns></returns>
    public static string CreateEndSessionUrl(this RequestUrl request,
        string? idTokenHint = null,
        string? postLogoutRedirectUri = null,
        string? state = null,
        Parameters? extra = null)
    {
        var values = new Parameters();

        values.AddOptional(OidcConstants.EndSessionRequest.IdTokenHint, idTokenHint);
        values.AddOptional(OidcConstants.EndSessionRequest.PostLogoutRedirectUri, postLogoutRedirectUri);
        values.AddOptional(OidcConstants.EndSessionRequest.State, state);

        return request.Create(values.Merge(extra));
    }
}