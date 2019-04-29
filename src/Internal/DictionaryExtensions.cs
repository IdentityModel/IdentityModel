// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace IdentityModel.Internal
{
    internal static class DictionaryExtensions
    {
        public static void AddOptional(this IDictionary<string, string> parameters, string key, string value)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            if (value.IsPresent())
            {
                if (parameters.ContainsKey(key))
                {
                    throw new InvalidOperationException($"Duplicate parameter: {key}");
                }
                else
                {
                    parameters.Add(key, value);
                }
            }
        }

        public static void AddRequired(this IDictionary<string, string> parameters, string key, string value, bool allowEmpty = false)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            if (value.IsPresent())
            {
                if (parameters.ContainsKey(key))
                {
                    throw new InvalidOperationException($"Duplicate parameter: {key}");
                }
                else
                {
                    parameters.Add(key, value);
                }
            }
            else
            {
                if (allowEmpty)
                {
                    parameters.Add(key, "");
                }
                else
                {
                    throw new ArgumentException("Parameter is required", key);
                }
            }
        }
    }
}