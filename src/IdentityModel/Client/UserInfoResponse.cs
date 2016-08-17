// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace IdentityModel.Client
{
    public class UserInfoResponse
    {
        public string Raw { get; }
        public JObject JsonObject { get; }
        public IEnumerable<Claim> Claims { get; }

        public bool IsHttpError { get; }
        public HttpStatusCode HttpErrorStatusCode { get; }
        public string HttpErrorReason { get; }

        public bool IsError { get; }
        public string ErrorMessage { get; }

        public UserInfoResponse(string raw)
        {
            Raw = raw;

            try
            {
                JsonObject = JObject.Parse(raw);
                var claims = new List<Claim>();

                foreach (var x in JsonObject)
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
    }
}