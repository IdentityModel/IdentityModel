// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<Claim> Claims
        {
            get
            {
                if (!IsError) return Json.ToClaims();

                return Enumerable.Empty<Claim>();
            }
        }
    }
}