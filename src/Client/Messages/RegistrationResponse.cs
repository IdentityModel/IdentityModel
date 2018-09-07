// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net;

#pragma warning disable 1591

namespace IdentityModel.Client
{
    /// <summary>
    /// Models an OpenID Connect dynamic client registration response
    /// </summary>
    /// <seealso cref="IdentityModel.Client.Response" />
    public class RegistrationResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationResponse"/> class.
        /// </summary>
        /// <param name="raw">The raw response data.</param>
        public RegistrationResponse(string raw) : base(raw)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationResponse"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public RegistrationResponse(Exception exception) : base(exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reason">The reason.</param>
        public RegistrationResponse(HttpStatusCode statusCode, string reason) : base(statusCode, reason)
        {
        }

        public string ErrorDescription         => Json.TryGetString("error_description");
        public string ClientId                 => Json.TryGetString(OidcConstants.RegistrationResponse.ClientId);
        public string ClientSecret             => Json.TryGetString(OidcConstants.RegistrationResponse.ClientSecret);
        public string RegistrationAccessToken  => Json.TryGetString(OidcConstants.RegistrationResponse.RegistrationAccessToken);
        public string RegistrationClientUri    => Json.TryGetString(OidcConstants.RegistrationResponse.RegistrationClientUri);
        public int? ClientIdIssuedAt           => Json.TryGetInt(OidcConstants.RegistrationResponse.ClientIdIssuedAt);
        public int? ClientSecretExpiresAt      => Json.TryGetInt(OidcConstants.RegistrationResponse.ClientSecretExpiresAt);
    }
}