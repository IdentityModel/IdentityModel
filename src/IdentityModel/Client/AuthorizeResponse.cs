// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;

namespace IdentityModel.Client
{
    /// <summary>
    /// Models the response of an authorize request
    /// </summary>
    public class AuthorizeResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeResponse"/> class.
        /// </summary>
        /// <param name="raw">The raw response URL.</param>
        public AuthorizeResponse(string raw)
        {
            Raw = raw;
            ParseRaw();
        }

        /// <summary>
        /// Gets the raw response URL.
        /// </summary>
        /// <value>
        /// The raw.
        /// </value>
        public string Raw { get; }

        /// <summary>
        /// Gets the key/value pairs of the response.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public Dictionary<string, string> Values { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the authorization code.
        /// </summary>
        /// <value>
        /// The authorization code.
        /// </value>
        public string Code => TryGet(OidcConstants.AuthorizeResponse.Code);

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken => TryGet(OidcConstants.AuthorizeResponse.AccessToken);

        /// <summary>
        /// Gets the identity token.
        /// </summary>
        /// <value>
        /// The identity token.
        /// </value>
        public string IdentityToken => TryGet(OidcConstants.AuthorizeResponse.IdentityToken);

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error => TryGet(OidcConstants.AuthorizeResponse.Error);

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope => TryGet(OidcConstants.AuthorizeResponse.Scope);

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        public string TokenType => TryGet(OidcConstants.AuthorizeResponse.TokenType);

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State => TryGet(OidcConstants.AuthorizeResponse.State);

        /// <summary>
        /// Gets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription => TryGet(OidcConstants.AuthorizeResponse.ErrorDescription);

        /// <summary>
        /// Gets a value indicating whether the response is an error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the response is an error; otherwise, <c>false</c>.
        /// </value>
        public bool IsError => !string.IsNullOrEmpty(Error);

        /// <summary>
        /// Gets the expires in.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        public long ExpiresIn
        {
            get
            {
                var value = TryGet(OidcConstants.AuthorizeResponse.ExpiresIn);

                long longValue;
                long.TryParse(value, out longValue);

                return longValue;
            }
        }

        private void ParseRaw()
        {
            string[] fragments;

            // query string encoded
            if (Raw.Contains("?"))
            {
                fragments = Raw.Split('?');

                var additionalHashFragment = fragments[1].IndexOf('#');
                if (additionalHashFragment >= 0)
                {
                    fragments[1] = fragments[1].Substring(0, additionalHashFragment);
                }
            }
            // fragment encoded
            else if (Raw.Contains("#"))
            {
                fragments = Raw.Split('#');
            }
            // form encoded
            else
            {
                fragments = new[] { "", Raw };
            }

            var qparams = fragments[1].Split('&');

            foreach (var param in qparams)
            {
                var parts = param.Split('=');

                if (parts.Length == 2)
                {
                    Values.Add(parts[0], parts[1]);
                }
                else
                {
                    throw new InvalidOperationException("Malformed callback URL.");
                }
            }
        }

        /// <summary>
        /// Tries the get a value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string TryGet(string type)
        {
            string value;
            if (Values.TryGetValue(type, out value))
            {
                return WebUtility.UrlDecode(value);
            }

            return null;
        }
    }
}