// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Internal
{
    internal static class DictionaryExtensions
    {
        public static void AddIfPresent(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (value.IsPresent()) dictionary.Add(key, value);
        }
    }
}