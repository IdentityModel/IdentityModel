// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityModel.Client;

/// <summary>
/// Extensions for JObject
/// </summary>
public static class JsonElementExtensions
{
    /// <summary>
    /// Converts a JSON claims object to a list of Claim
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="issuer">Optional issuer name to add to claims.</param>
    /// <param name="excludeKeys">Claims that should be excluded.</param>
    /// <returns></returns>

    public static IEnumerable<Claim> ToClaims(this JsonElement json, string? issuer = null, params string[] excludeKeys)
    {
        var claims = new List<Claim>();
        var excludeList = excludeKeys.ToList();

        foreach (var x in json.EnumerateObject())
        {
            if (excludeList.Contains(x.Name)) continue;
                
            if (x.Value.ValueKind  == JsonValueKind.Array)
            {
                foreach (var item in x.Value.EnumerateArray())
                {
                    claims.Add(new Claim(x.Name, Stringify(item), ClaimValueTypes.String, issuer));
                }
            }
            else
            {
                claims.Add(new Claim(x.Name, Stringify(x.Value), ClaimValueTypes.String, issuer));
            }
        }

        return claims;
    }

    private static string Stringify(JsonElement item)
    {
        // String is special because item.ToString(Formatting.None) will result in "/"string/"". The quotes will be added.
        // Boolean needs item.ToString otherwise 'true' => 'True'
        var value = item.ValueKind == JsonValueKind.String ?
            item.ToString() :
            item.GetRawText();

        return value;
    }

    /// <summary>
    /// Tries to get a value from a JObject
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static JsonElement TryGetValue(this JsonElement json, string name)
    {
        if (json.ValueKind == JsonValueKind.Undefined)
        {
            return default;
        }

        return json.TryGetProperty(name, out JsonElement value) ? value : default;
    }

    /// <summary>
    /// Tries to get an int from a JObject
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static int? TryGetInt(this JsonElement json, string name)
    {
        var value = json.TryGetString(name);

        if (value != null)
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
        }

        return null;
    }

    /// <summary>
    /// Tries to get a string from a JObject
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static string? TryGetString(this JsonElement json, string name)
    {
        JsonElement value = json.TryGetValue(name);
        return value.ValueKind == JsonValueKind.Undefined ? null : value.ToString();
    }

    /// <summary>
    /// Tries to get a boolean from a JObject
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static bool? TryGetBoolean(this JsonElement json, string name)
    {
        var value = json.TryGetString(name);

        if (bool.TryParse(value, out bool result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Tries to get a string array from a JObject
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static IEnumerable<string> TryGetStringArray(this JsonElement json, string name)
    {
        var values = new List<string>();

        var array = json.TryGetValue(name);
        if (array.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in array.EnumerateArray())
            {
                values.Add(item.ToString());
            }
        }

        return values;
    }
}