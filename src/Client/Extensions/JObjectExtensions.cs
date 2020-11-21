﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityModel.Client
{
    /// <summary>
    /// Extensions for JObject
    /// </summary>
    public static class JObjectExtensions
    {
        /// <summary>
        /// Converts a JSON claims object to a list of Claim
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="issuer">Optional issuer name to add to claims.</param>
        /// <param name="excludeKeys">Claims that should be excluded.</param>
        /// <returns></returns>
        public static IEnumerable<Claim> ToClaims(this JObject json, string issuer = null, params string[] excludeKeys)
        {
            var claims = new List<Claim>();
            var excludeList = excludeKeys.ToList();

            foreach (var x in json)
            {
                if (excludeList.Contains(x.Key)) continue;

                if (x.Value is JArray array)
                {
                    foreach (var item in array)
                    {
                        claims.Add(new Claim(x.Key, Stringify(item), ClaimValueTypes.String, issuer));
                    }
                }
                else
                {
                    claims.Add(new Claim(x.Key, Stringify(x.Value), ClaimValueTypes.String, issuer));
                }
            }

            return claims;
        }

        private static string Stringify(JToken item)
        {
            // String is special because item.ToString(Formatting.None) will result in "/"string/"". The quotes will be added.
            // Boolean needs item.ToString otherwise 'true' => 'True'
            var value = item.Type == JTokenType.String ?
                item.ToString() :
                item.ToString(Newtonsoft.Json.Formatting.None);

            return value;
        }

        /// <summary>
        /// Tries to get a value from a JObject
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static JToken TryGetValue(this JObject json, string name)
        {
            if (json != null && json.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out JToken value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Tries to get an int from a JObject
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static int? TryGetInt(this JObject json, string name)
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
        public static string TryGetString(this JObject json, string name)
        {
            JToken value = json.TryGetValue(name);
            return value?.ToString();
        }

        /// <summary>
        /// Tries to get a boolean from a JObject
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static bool? TryGetBoolean(this JObject json, string name)
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
        public static IEnumerable<string> TryGetStringArray(this JObject json, string name)
        {
            var values = new List<string>();

            if (json.TryGetValue(name) is JArray array)
            {
                foreach (var item in array)
                {
                    values.Add(item.ToString());
                }
            }

            return values;
        }
    }
}