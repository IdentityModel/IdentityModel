// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace IdentityModel.Client
{
    public class IntrospectionResponse
    {
        public string Raw { get; set; }

        public bool IsError { get; set; }
        public string Error { get; set; }

        public bool IsActive { get; set; }
        public IEnumerable<Tuple<string, string>> Claims { get; set; }

        public IntrospectionResponse()
        { }

        public IntrospectionResponse(string raw)
        {
            Raw = raw;

            try
            {
                var json = JObject.Parse(raw);
                IsActive = bool.Parse(json["active"].ToString());

                var claims = new List<Tuple<string, string>>();

                foreach (var x in json)
                {
                    var array = x.Value as JArray;

                    if (array != null)
                    {
                        foreach (var item in array)
                        {
                            claims.Add(Tuple.Create(x.Key, item.ToString()));
                        }
                    }
                    else
                    {
                        claims.Add(Tuple.Create(x.Key, x.Value.ToString()));
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