// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdentityModel.Jwk;

/// <summary>
/// Contains a collection of <see cref="JsonWebKey"/> that can be populated from a json string.
/// </summary>
public class JsonWebKeySet
{
    /// <summary>
    /// Initializes an new instance of <see cref="JsonWebKeySet"/>.
    /// </summary>
    public JsonWebKeySet()
    { }

    /// <summary>
    /// Initializes an new instance of <see cref="JsonWebKeySet"/> from a json string.
    /// </summary>
    /// <param name="json">a json string containing values.</param>
    /// <exception cref="InvalidOperationException">if web keys are malformed</exception>
    /// <exception cref="ArgumentNullException">if 'json' is null or whitespace.</exception>
    public JsonWebKeySet(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));

        var jwebKeys = JsonSerializer.Deserialize<JsonWebKeySet>(json, JwkSourceGenerationContext.Default.JsonWebKeySet);
        if (jwebKeys == null) throw new InvalidOperationException("invalid JSON web keys");
        
        Keys = jwebKeys.Keys;
        RawData = json;
    }

    /// <summary>
    /// A list of JSON web keys
    /// </summary>
    [JsonPropertyName("keys")]
    public List<JsonWebKey> Keys { get; set; } = new();
    
    /// <summary>
    /// The JSON string used to deserialize this object
    /// </summary>
    [JsonIgnore]
    public string? RawData { get; set; }
}
