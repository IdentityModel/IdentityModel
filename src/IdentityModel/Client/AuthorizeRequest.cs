// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IdentityModel.Client
{
    /// <summary>
    /// Helper class for creating authorize request URLs
    /// </summary>
    public class AuthorizeRequest
    {
        private readonly Uri _authorizeEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRequest"/> class.
        /// </summary>
        /// <param name="authorizeEndpoint">The authorize endpoint.</param>
        public AuthorizeRequest(Uri authorizeEndpoint)
        {
            _authorizeEndpoint = authorizeEndpoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRequest"/> class.
        /// </summary>
        /// <param name="authorizeEndpoint">The authorize endpoint.</param>
        public AuthorizeRequest(string authorizeEndpoint)
        {
            _authorizeEndpoint = new Uri(authorizeEndpoint);
        }

        /// <summary>
        /// Creates URL based on key/value input pairs.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public string Create(IDictionary<string, string> values)
        {
            var qs = string.Join("&", values.Select(kvp => string.Format("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value))).ToArray());

            if (_authorizeEndpoint.IsAbsoluteUri)
            {
                return string.Format("{0}?{1}", _authorizeEndpoint.AbsoluteUri, qs);
            }
            else
            {
                return string.Format("{0}?{1}", _authorizeEndpoint.OriginalString, qs);
            }
        }
    }
}