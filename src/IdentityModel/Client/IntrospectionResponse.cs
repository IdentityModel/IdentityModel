// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityModel.Client
{
    public class IntrospectionResponse
    {
        public string Raw { get; }
        public JObject Json { get; }

        public bool IsError { get; set; }
        public string Error { get; set; }

        public bool IsActive { get; }
        public IEnumerable<Claim> Claims { get; }

        public IntrospectionResponse()
        { }

        public IntrospectionResponse(string raw)
        {
            Raw = raw;

            try
            {
                Json = JObject.Parse(raw);
                IsActive = bool.Parse(Json["active"].ToString());
                Claims = Json.ToClaims();
            }
            catch (Exception ex)
            {
                IsError = true;
                Error = ex.ToString();
            }
        }
    }
}