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
                var json = JObject.Parse(raw);
                IsActive = bool.Parse(json["active"].ToString());

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

                Claims = claims;
            }
            catch (Exception ex)
            {
                IsError = true;
                Error = ex.ToString();
            }
        }
    }
}