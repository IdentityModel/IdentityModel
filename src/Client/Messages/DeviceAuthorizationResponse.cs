// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

/// <summary>
/// Models an OAuth device authorization response
/// </summary>
/// <seealso cref="IdentityModel.Client.ProtocolResponse" />
public class DeviceAuthorizationResponse : ProtocolResponse
{
    /// <summary>
    /// Gets the device verification code.
    /// </summary>
    /// <value>
    /// The device code.
    /// </value>
    public string? DeviceCode => Json.TryGetString(OidcConstants.DeviceAuthorizationResponse.DeviceCode);

    /// <summary>
    /// Gets the end-user verification code.
    /// </summary>
    /// <value>
    /// The user code.
    /// </value>
    public string? UserCode => Json.TryGetString(OidcConstants.DeviceAuthorizationResponse.UserCode);

    /// <summary>
    /// Gets the end-user verification URI on the authorization server.The URI should be short and easy to remember as end users will be asked to manually type it into their user-agent.
    /// </summary>
    /// <value>
    /// The verification URI.
    /// </value>
    public string? VerificationUri => Json.TryGetString(OidcConstants.DeviceAuthorizationResponse.VerificationUri);

    /// <summary>
    /// Gets the verification URI that includes the "user_code" (or other information with the same function as the "user_code"), designed for non-textual transmission.
    /// </summary>
    /// <value>
    /// The complete verification URI.
    /// </value>
    public string? VerificationUriComplete => Json.TryGetString(OidcConstants.DeviceAuthorizationResponse.VerificationUriComplete);

    /// <summary>
    /// Gets the lifetime in seconds of the "device_code" and "user_code".
    /// </summary>
    /// <value>
    /// The expires in.
    /// </value>
    public int? ExpiresIn => Json.TryGetInt(OidcConstants.DeviceAuthorizationResponse.ExpiresIn);

    /// <summary>
    /// Gets the minimum amount of time in seconds that the client SHOULD wait between polling requests to the token endpoint. If no value is provided, clients MUST use 5 as the default.
    /// </summary>
    /// <value>
    /// The interval.
    /// </value>
    public int Interval => Json.TryGetInt(OidcConstants.DeviceAuthorizationResponse.Interval) ?? 5;

    /// <summary>
    /// Gets the error description.
    /// </summary>
    /// <value>
    /// The error description.
    /// </value>
    public string? ErrorDescription => Json.TryGetString(OidcConstants.TokenResponse.ErrorDescription);
}