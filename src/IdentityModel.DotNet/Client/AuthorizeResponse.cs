/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
                return TryGet(OAuth2Constants.Code);
            }
        }

        public string AccessToken
        {
            get
            {
                return TryGet(OAuth2Constants.AccessToken);
            }
        }

        public string IdentityToken
        {
            get
            {
                return TryGet(OAuth2Constants.IdentityToken);
            }
        }

        public bool IsError { get; internal set; }

        public string Error
        {
            get
            {
                return TryGet(OAuth2Constants.Error);
            }
        }

        public long ExpiresIn
        {
            get
            {
                var value = TryGet(OAuth2Constants.ExpiresIn);

                long longValue = 0;
                long.TryParse(value, out longValue);

                return longValue;
            }
        }

        public string Scope
        {
            get
            {
                return TryGet(OAuth2Constants.Scope);
            }
        }

        public string TokenType
        {
            get
            {
                return TryGet(OAuth2Constants.TokenType);
            }
        }

        public string State
        {
            get
            {
                return TryGet(OAuth2Constants.State);
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

            if (Raw.Contains(OAuth2Constants.Error))
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