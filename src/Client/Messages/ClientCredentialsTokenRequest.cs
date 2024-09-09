// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client;

/// <summary>
/// Request for token using client_credentials
/// </summary>
/// <seealso cref="TokenRequest" />
public class ClientCredentialsTokenRequest : TokenRequest
{
    /// <summary>
    /// Space separated list of the requested scopes 
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public string? Scope { get; set; }
        
    /// <summary>
    /// List of requested resources
    /// </summary>
    /// <value>
    /// The scope.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}