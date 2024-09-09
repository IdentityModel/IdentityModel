// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityModel.Client;

/// <summary>
/// Models an OpenID Connect userinfo response
/// </summary>
/// <seealso cref="IdentityModel.Client.ProtocolResponse" />
public class UserInfoResponse : ProtocolResponse
{
    /// <summary>
    /// Allows to initialize instance specific data.
    /// </summary>
    /// <param name="initializationData">The initialization data.</param>
    /// <returns></returns>
    protected override Task InitializeAsync(object? initializationData = null)
    {
        if (!IsError && Json.HasValue)
        {
            Claims = Json.Value.ToClaims();
        }
        else
        {
            Claims = Enumerable.Empty<Claim>();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the claims.
    /// </summary>
    /// <value>
    /// The claims.
    /// </value>
    public IEnumerable<Claim> Claims { get; private set; } = default!;
}