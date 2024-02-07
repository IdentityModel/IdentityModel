// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

/// <summary>
/// Models parameters that can be included when redirecting to the authorize
/// endpoint when not using a JWT-Secured Authorization Request (JAR).
/// </summary>
public class AuthorizeRedirectParameters : IAuthorizeRequestParameters
{
    /// <summary>
    /// ctor
    /// </summary>
    public AuthorizeRedirectParameters(string clientId, string responseType)
    {
        ClientId = clientId;
        ResponseType = responseType;
    }

    /// <inheritdoc />
    public string ClientId { get; set; } 
    /// <inheritdoc />
    public string ResponseType { get; set; }
    /// <inheritdoc />
    public string? Scope { get; set; }
    /// <inheritdoc />
    public string? RedirectUri { get; set; }
    /// <inheritdoc />
    public string? State { get; set; }
    /// <inheritdoc />
    public string? Nonce { get; set; }
    /// <inheritdoc />
    public string? LoginHint { get; set; }
    /// <inheritdoc />
    public string? AcrValues { get; set; }
    /// <inheritdoc />
    public string? Prompt { get; set; }
    /// <inheritdoc />
    public string? ResponseMode { get; set; }
    /// <inheritdoc />
    public string? CodeChallenge { get; set; }
    /// <inheritdoc />
    public string? CodeChallengeMethod { get; set; }
    /// <inheritdoc />
    public string? Display { get; set; }
    /// <inheritdoc />
    public int? MaxAge { get; set; }
    /// <inheritdoc />
    public string? UiLocales { get; set; }
    /// <inheritdoc />
    public string? IdTokenHint { get; set; }
    /// <inheritdoc />
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
    /// <inheritdoc />
    public string? DPoPKeyThumbprint { get; set; }
}