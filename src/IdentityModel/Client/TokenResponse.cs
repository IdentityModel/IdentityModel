// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net;

namespace IdentityModel.Client
{
    /// <summary>
    /// Models a response from an OpenID Connect/OAuth 2 token endpoint
    /// </summary>
    /// <seealso cref="IdentityModel.Client.Response" />
    public class TokenResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResponse"/> class.
        /// </summary>
        /// <param name="raw">The raw response data.</param>
        public TokenResponse(string raw) : base(raw)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResponse"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public TokenResponse(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reason">The reason.</param>
        public TokenResponse(HttpStatusCode statusCode, string reason) : base(statusCode, reason)
        {
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken => TryGet(OidcConstants.TokenResponse.AccessToken);

        /// <summary>
        /// Gets the identity token.
        /// </summary>
        /// <value>
        /// The identity token.
        /// </value>
        public string IdentityToken => TryGet(OidcConstants.TokenResponse.IdentityToken);

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        public string TokenType => TryGet(OidcConstants.TokenResponse.TokenType);

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken => TryGet(OidcConstants.TokenResponse.RefreshToken);

        /// <summary>
        /// Gets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription => TryGet(OidcConstants.TokenResponse.ErrorDescription);

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
                var value = TryGet(OidcConstants.TokenResponse.ExpiresIn);

                if (value != null)
                {
                    if (long.TryParse(value, out long longValue))
                    {
                        return longValue;
                    }
                }

                return 0;
            }
        }
    }
}