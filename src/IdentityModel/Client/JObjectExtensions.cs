// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityModel.Client
{
    public static class JObjectExtensions
    {
        public static IEnumerable<Claim> ToClaims(this JObject json)
        {
            var claims = new List<Claim>();

            foreach (var x in json)
            {
                var array = x.Value as JArray;

                if (array != null)
                {
                    foreach (var item in array)
                    {
                        claims.Add(new Claim(x.Key, item.ToString()));
                    }
                }
                else
                {
                    claims.Add(new Claim(x.Key, x.Value.ToString()));
                }
            }

            return claims;
        }

        public static JToken TryGetValue(this JObject json, string name)
        {
            JToken value;
            if (json != null && json.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out value))
            {
                return value;
            }

            return null;
        }

        public static int? TryGetInt(this JObject json, string name)
        {
            var value = json.TryGetString(name);

            if (value != null)
            {
                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    return intValue;
                }
            }

            return null;
        }

        public static string TryGetString(this JObject json, string name)
        {
            JToken value = json.TryGetValue(name);
            return value?.ToString() ?? null;
        }

        public static bool? TryGetBoolean(this JObject json, string name)
        {
            var value = json.TryGetString(name);

            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static IEnumerable<string> TryGetStringArray(this JObject json, string name)
        {
            var values = new List<string>();

            var array = json.TryGetValue(name) as JArray;
            if (array != null)
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