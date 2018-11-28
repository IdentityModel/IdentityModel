// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace IdentityModel.Internal
{
    internal static class DictionaryExtensions
    {
        public static void AddIfPresent(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (value.IsPresent()) dictionary.Add(key, value);
        }

        public static void AddOptional(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (value.IsPresent())
            {
                if (dictionary.ContainsKey(key))
                {
                    throw new InvalidOperationException($"Duplicate parameter: {key}");
                }
                else
                {
                    dictionary.Add(key, value);
                }
            }
        }

        public static void AddRequired(this IDictionary<string, string> dictionary, string key, string value, bool allowEmpty = false)
        {
            if (value.IsPresent())
            {
                if (dictionary.ContainsKey(key))
                {
                    throw new InvalidOperationException($"Duplicate parameter: {key}");
                }
                else
                {
                    dictionary.Add(key, value);
                }
            }
            else
            {
                if (allowEmpty)
                {
                    dictionary.Add(key, "");
                }
                else
                {
                    throw new ArgumentException("Parameter is required", key);
                }
            }
        }
    }
}