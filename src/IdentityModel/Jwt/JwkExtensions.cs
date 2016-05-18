// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Newtonsoft.Json;
using System.Text;

namespace IdentityModel.Jwt
{
    public static class JsonWebKeyExtensions
    {
        public static string ToJwkString(this JsonWebKey key)
        {
            var json = JsonConvert.SerializeObject(key);            
            return Base64Url.Encode(Encoding.ASCII.GetBytes(json));
        }
    }
}