// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace IdentityModel.Client
{
    public class IntrospectionResponse : Response
    {
        public IntrospectionResponse(string raw) : base(raw)
        {
            if (!IsError)
            {
                var claims = Json.ToClaims(excludeKeys: "scope").ToList();

                // due to a bug in identityserver - we need to be able to deal with the scope list both in array as well as space-separated list format
                var scope = Json.TryGetValue("scope");

                // scope element exists
                if (scope != null)
                {
                    // it's an array
                    var scopeArray = scope as JArray;
                    if (scopeArray != null)
                    {
                        foreach (var item in scopeArray)
                        {
                            claims.Add(new Claim("scope", item.ToString()));
                        }
                    }
                    else
                    {
                        // it's a string
                        var scopeString = scope.ToString();

                        var scopes = scopeString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var scopeValue in scopes)
                        {
                            claims.Add(new Claim("scope", scopeValue));
                        }
                    }
                }

                Claims = claims;
            }
        }

        public IntrospectionResponse(Exception exception) : base(exception)
        {
        }

        public IntrospectionResponse(HttpStatusCode statusCode, string reason) : base(statusCode, reason)
        {
        }

        public bool IsActive => Json.TryGetBoolean("active").Value;
        public IEnumerable<Claim> Claims { get; }


    }
}