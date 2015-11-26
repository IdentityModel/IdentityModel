/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
                Error = ex.Message;
            }
        }
    }
}