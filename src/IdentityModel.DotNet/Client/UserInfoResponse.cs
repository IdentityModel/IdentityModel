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
using System.Net;

namespace IdentityModel.Client
{
    public class UserInfoResponse
    {
        public UserInfoResponse(string raw)
        {
            Raw = raw;

            try
            {
                JsonObject = JObject.Parse(raw);
                var claims = new List<Tuple<string, string>>();

                foreach (var x in JsonObject)
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
                ErrorMessage = ex.Message;
            }
        }

        public UserInfoResponse(HttpStatusCode statusCode, string httpErrorReason)
        {
            IsHttpError = true;
            IsError = true;
            HttpErrorStatusCode = statusCode;
            HttpErrorReason = httpErrorReason;
        }

        public string Raw { get; private set; }
        public JObject JsonObject { get; private set; }
        public IEnumerable<Tuple<string, string>> Claims { get; set; }

        public bool IsHttpError { get; private set; }
        public HttpStatusCode HttpErrorStatusCode { get; private set; }
        public string HttpErrorReason { get; private set; }
        
        public bool IsError { get; private set; }
        public string ErrorMessage { get; set; }
    }
}