// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace IdentityModel.Client
{
    /// <summary>
    /// Models an OAuth 2.0 token revocation response
    /// </summary>
    /// <seealso cref="IdentityModel.Client.Response" />
    public class TokenRevocationResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRevocationResponse"/> class.
        /// </summary>
        public TokenRevocationResponse()
            : base(HttpStatusCode.OK, "OK")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRevocationResponse"/> class.
        /// </summary>
        /// <param name="raw">The raw response data.</param>
        public TokenRevocationResponse(string raw) : base(raw)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRevocationResponse"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public TokenRevocationResponse(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRevocationResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reason">The reason.</param>
        public TokenRevocationResponse(HttpStatusCode statusCode, string reason) : base(statusCode, reason)
        {
        }
    }
}