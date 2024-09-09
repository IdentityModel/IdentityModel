// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Request for token using urn:openid:params:grant-type:ciba grant type
/// </summary>
/// <seealso cref="TokenRequest" />
public class BackchannelAuthenticationTokenRequest : TokenRequest
{
    /// <summary>
    /// REQUIRED. It is the unique identifier to identify the authentication request (transaction) made by the Client.
    /// </summary>
    public string AuthenticationRequestId { get; set; } = default!;
    
    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The resources.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}