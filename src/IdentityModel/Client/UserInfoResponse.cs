// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace IdentityModel.Client
{
    /// <summary>
    /// Models an OpenID Connect userinfo response
    /// </summary>
    /// <seealso cref="IdentityModel.Client.Response" />
    public class UserInfoResponse : Response
    {
        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <value>
        /// The claims.
        /// </value>
        public IEnumerable<Claim> Claims { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoResponse"/> class.
        /// </summary>
        /// <param name="raw">The raw response data.</param>
        public UserInfoResponse(string raw) : base(raw)
        {
            if (!IsError) Claims = Json.ToClaims();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoResponse"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public UserInfoResponse(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reason">The reason.</param>
        public UserInfoResponse(HttpStatusCode statusCode, string reason) : base(statusCode, reason)
        {
        }
    }
}