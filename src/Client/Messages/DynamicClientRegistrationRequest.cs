// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

/// <summary>
/// Request for dynamic client registration
/// </summary>
/// <seealso cref="ProtocolRequest" />
public class DynamicClientRegistrationRequest : ProtocolRequest
{
    /// <summary>
    /// Gets or sets the token.
    /// </summary>
    /// <value>
    /// The token.
    /// </value>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the registration request.
    /// </summary>
    /// <value>
    /// The registration request.
    /// </value>
    public DynamicClientRegistrationDocument Document { get; set; } = default!;
}