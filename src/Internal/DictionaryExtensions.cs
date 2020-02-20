// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001002f25809ad9fde9869a3ae4558b897c8a23458393921395b9439e03d6a52afadbf6ff65ef1049cd2ee4ca5501976ad45b453dc3780b8fa7eb39bae755163ef92d53403a0da484b79d24de1bb759eedceb1e13416c734d9c48b226fcd26c18e0a525b68cdba9f2395502d7df5a6d45c2478edd52752511e2924ea209f83aaa23a1")]
namespace IdentityModel.Internal
{
    internal static class DictionaryExtensions
    {
        public static void AddOptional(this IDictionary<string, string> parameters, string key, string value)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (key.IsMissing()) throw new ArgumentNullException(nameof(key));

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
            if (key.IsMissing()) throw new ArgumentNullException(nameof(key));
            
            if (parameters.ContainsKey(key))
            {
                throw new InvalidOperationException($"Duplicate parameter: {key}");
            }
            else if (value.IsPresent() || allowEmpty)
            {
                parameters.Add(key, value);
            }
            else
            {
                throw new ArgumentException($"Parameter is required", key);
            }
        }
    }
}
