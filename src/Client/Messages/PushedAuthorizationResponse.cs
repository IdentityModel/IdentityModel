// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

/// <summary>
/// Models the response from a Pushed Authorization Request
/// </summary>
public class PushedAuthorizationResponse : ProtocolResponse
{
    /// <summary>
    /// A JSON number that represents the lifetime of the request URI in seconds
    /// as a positive integer. The request URI lifetime is at the discretion of
    /// the authorization server but will typically be relatively short (e.g.,
    /// between 5 and 600 seconds).
    /// </summary>
    public int? ExpiresIn => 
        Json?.TryGetInt(OidcConstants.PushedAuthorizationRequestResponse.ExpiresIn);


    /// <summary>
    /// The request URI corresponding to the authorization request posted. This URI
    /// is a single-use reference to the respective request data in the subsequent
    /// authorization request. The way the authorization process obtains the
    /// authorization request data is at the discretion of the authorization server
    /// and is out of scope of this specification. There is no need to make the
    /// authorization request data available to other parties via this URI.
    /// </summary>
    public string? RequestUri =>
        Json?.TryGetString(OidcConstants.PushedAuthorizationRequestResponse.RequestUri);
}