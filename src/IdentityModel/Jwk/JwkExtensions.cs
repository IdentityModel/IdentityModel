// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Newtonsoft.Json;
using System.Text;

namespace IdentityModel.Jwk
{
    /// <summary>
    /// Extensions for JsonWebKey
    /// </summary>
    public static class JsonWebKeyExtensions
    {
        /// <summary>
        /// Converts a JSON web key to a URL safe string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string ToJwkString(this JsonWebKey key)
        {
            var json = JsonConvert.SerializeObject(key);            
            return Base64Url.Encode(Encoding.UTF8.GetBytes(json));
        }
    }
}