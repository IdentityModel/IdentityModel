// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net;

namespace IdentityModel.Client
{
    public class TokenResponse : Response
    {
        public TokenResponse(string raw) : base(raw)
        {
        }

        public TokenResponse(Exception exception) : base(exception)
        {
        }

        public TokenResponse(HttpStatusCode statusCode, string reason) : base(statusCode, reason)
        {
        }

        public string AccessToken      => TryGet(OidcConstants.TokenResponse.AccessToken);
        public string IdentityToken    => TryGet(OidcConstants.TokenResponse.IdentityToken);
        public string TokenType        => TryGet(OidcConstants.TokenResponse.TokenType);
        public string RefreshToken     => TryGet(OidcConstants.TokenResponse.RefreshToken);
        public string ErrorDescription => TryGet(OidcConstants.TokenResponse.ErrorDescription);

        public long ExpiresIn
        {
            get
            {
                var value = TryGet(OidcConstants.TokenResponse.ExpiresIn);

                if (value != null)
                {
                    long longValue;
                    if (long.TryParse(value, out longValue))
                    {
                        return longValue;
                    }
                }

                return 0;
            }
        }
    }
}