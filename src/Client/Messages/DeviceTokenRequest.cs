// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

/// <summary>
/// Request for token using urn:ietf:params:oauth:grant-type:device_code
/// </summary>
/// <seealso cref="TokenRequest" />
public class DeviceTokenRequest : TokenRequest
{
    /// <summary>
    /// Gets or sets the device code.
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public string DeviceCode { get; set; } = default!;
}