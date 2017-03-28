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
    /// <summary>
    /// Models an OAuth 2.0 introspection response
    /// </summary>
    /// <seealso cref="IdentityModel.Client.Response" />
    public class IntrospectionResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionResponse"/> class.
        /// </summary>
        /// <param name="raw">The raw response data.</param>
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
                    if (scope is JArray scopeArray)
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

        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionResponse"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public IntrospectionResponse(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntrospectionResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reason">The reason.</param>
        public IntrospectionResponse(HttpStatusCode statusCode, string reason) : base(statusCode, reason)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the token is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the token is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive => Json.TryGetBoolean("active").Value;

        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <value>
        /// The claims.
        /// </value>
        public IEnumerable<Claim> Claims { get; }
    }
}