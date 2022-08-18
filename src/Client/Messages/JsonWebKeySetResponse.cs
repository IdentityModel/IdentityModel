// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using IdentityModel.Jwk;

namespace IdentityModel.Client;

/// <summary>
/// Models a response from a JWK endpoint
/// </summary>
/// <seealso cref="IdentityModel.Client.ProtocolResponse" />
public class JsonWebKeySetResponse : ProtocolResponse
{
    /// <summary>
    /// Intializes the key set
    /// </summary>
    /// <param name="initializationData"></param>
    /// <returns></returns>
    protected override Task InitializeAsync(object? initializationData = null)
    {
        if (!HttpResponse.IsSuccessStatusCode)
        {
            ErrorMessage = initializationData as string;
        }
        else
        {
            KeySet = new JsonWebKeySet(Raw!);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// The key set
    /// </summary>
    public JsonWebKeySet? KeySet { get; set; }
}