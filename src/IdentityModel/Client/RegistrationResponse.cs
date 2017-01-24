// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net;

namespace IdentityModel.Client
{
    public class RegistrationResponse : Response
    {
        public RegistrationResponse(string raw) : base(raw)
        {
        }

        public RegistrationResponse(Exception exception) : base(exception)
        {
        }

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