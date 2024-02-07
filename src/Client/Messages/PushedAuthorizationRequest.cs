// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Models the parameters that can be pushed in a Pushed Authorization Request.
/// </summary>
/// <seealso cref="ProtocolRequest" />
public class PushedAuthorizationRequest : ProtocolRequest, IAuthorizeRequestParameters
{
    /// <summary>
    /// ctor
    /// </summary>
    public PushedAuthorizationRequest(string clientId, string responseType)
    {
        ClientId = clientId;
        ResponseType = responseType;
    }

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

/// <summary>
/// Models the parameters that can be pushed in a Pushed Authorization Request
/// as a JWT-Secure Authorization Request.
/// </summary>
/// <seealso cref="ProtocolRequest" />
public class PushedAuthorizationRequestWithRequestObject : ProtocolRequest
{
    /// <summary>
    /// ctor
    /// </summary>
    public PushedAuthorizationRequestWithRequestObject(string clientId, string request)
    {
        ClientId = clientId;
        Request = request;
    }

    /// <summary>
    /// Gets or sets the request protocol parameter.
    /// </summary>
    public string Request { get; set; }
}