// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;

namespace IdentityModel.Client
{
    public class AuthorizeResponse
    {
        public AuthorizeResponse(string raw)
        {
            Raw = raw;
            ParseRaw();
        }

        public string Raw { get; }
        public Dictionary<string, string> Values { get; } = new Dictionary<string, string>();
        
        public string Code             => TryGet(OidcConstants.AuthorizeResponse.Code);
        public string AccessToken      => TryGet(OidcConstants.AuthorizeResponse.AccessToken);
        public string IdentityToken    => TryGet(OidcConstants.AuthorizeResponse.IdentityToken);
        public string Error            => TryGet(OidcConstants.AuthorizeResponse.Error);
        public string Scope            => TryGet(OidcConstants.AuthorizeResponse.Scope);
        public string TokenType        => TryGet(OidcConstants.AuthorizeResponse.TokenType);
        public string State            => TryGet(OidcConstants.AuthorizeResponse.State);
        public string ErrorDescription => TryGet(OidcConstants.AuthorizeResponse.ErrorDescription);
        public bool IsError            => !string.IsNullOrEmpty(Error);

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