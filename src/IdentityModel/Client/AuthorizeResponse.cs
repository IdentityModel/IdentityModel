// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;

namespace IdentityModel.Client
{
    public class AuthorizeResponse
    {
        public string Raw { get; protected set; }
        public Dictionary<string, string> Values { get; protected set; }

        public string Code
        {
            get
            {
                return TryGet(OidcConstants.AuthorizeResponse.Code);
            }
        }

        public string AccessToken
        {
            get
            {
                return TryGet(OidcConstants.AuthorizeResponse.AccessToken);
            }
        }

        public string IdentityToken
        {
            get
            {
                return TryGet(OidcConstants.AuthorizeResponse.IdentityToken);
            }
        }

        public bool IsError { get; internal set; }

        public string Error
        {
            get
            {
                return TryGet(OidcConstants.AuthorizeResponse.Error);
            }
        }

        public long ExpiresIn
        {
            get
            {
                var value = TryGet(OidcConstants.AuthorizeResponse.ExpiresIn);

                long longValue = 0;
                long.TryParse(value, out longValue);

                return longValue;
            }
        }

        public string Scope
        {
            get
            {
                return TryGet(OidcConstants.AuthorizeResponse.Scope);
            }
        }

        public string TokenType
        {
            get
            {
                return TryGet(OidcConstants.AuthorizeResponse.TokenType);
            }
        }

        public string State
        {
            get
            {
                return TryGet(OidcConstants.AuthorizeResponse.State);
            }
        }

        public AuthorizeResponse(string raw)
        {
            Raw = raw;
            Values = new Dictionary<string, string>();
            ParseRaw();
        }

        private void ParseRaw()
        {
            var queryParameters = new Dictionary<string, string>();
            string[] fragments = null;

            // fragment encoded
            if (Raw.Contains("#"))
            {
                fragments = Raw.Split('#');
            }
            // query string encoded
            else if (Raw.Contains("?"))
            {
                fragments = Raw.Split('?');
            }
            // form encoded
            else
            {
                fragments = new string[] { "", Raw };
            }

            if (Raw.Contains(OidcConstants.AuthorizeResponse.Error))
            {
                IsError = true;
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

        private string TryGet(string type)
        {
            string value;
            if (Values.TryGetValue(type, out value))
            {
                return value;
            }

            return null;
        }
    }
}