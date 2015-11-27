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

using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace IdentityModel.Client
{
    public class TokenResponse
    {
        public string Raw { get; protected set; }
        public JObject Json { get; protected set; }

        private bool _isHttpError;
        private HttpStatusCode _httpErrorstatusCode;
        private string _httpErrorReason;

        public TokenResponse(string raw)
        {
            Raw = raw;

            try
            {
                Json = JObject.Parse(raw);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Invalid JSON response", ex);
            }
        }

        public TokenResponse(HttpStatusCode statusCode, string reason)
        {
            _isHttpError = true;
            _httpErrorstatusCode = statusCode;
            _httpErrorReason = reason;
        }

        public bool IsHttpError
        {
            get
            {
                return _isHttpError;
            }
        }

        public HttpStatusCode HttpErrorStatusCode
        {
            get
            {
                return _httpErrorstatusCode;
            }
        }

        public string HttpErrorReason
        {
            get
            {
                return _httpErrorReason;
            }
        }

        public string AccessToken
        {
            get
            {
                return GetStringOrNull(OAuth2Constants.AccessToken);
            }
        }

        public string IdentityToken
        {
            get
            {
                return GetStringOrNull(OAuth2Constants.IdentityToken);
            }
        }

        public string Error
        {
            get
            {
                return GetStringOrNull(OAuth2Constants.Error);
            }
        }

        public bool IsError
        {
            get
            {
                return (IsHttpError ||
                        !string.IsNullOrWhiteSpace(GetStringOrNull(OAuth2Constants.Error)));
            }
        }

        public long ExpiresIn
        {
            get
            {
                return GetLongOrNull(OAuth2Constants.ExpiresIn);
            }
        }

        public string TokenType
        {
            get
            {
                return GetStringOrNull(OAuth2Constants.TokenType);
            }
        }

        public string RefreshToken
        {
            get
            {
                return GetStringOrNull(OAuth2Constants.RefreshToken);
            }
        }

        protected virtual string GetStringOrNull(string name)
        {
            JToken value;
            if (Json != null && Json.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out value))
            {
                return value.ToString();
            }

            return null;
        }

        protected virtual long GetLongOrNull(string name)
        {
            JToken value;
            if (Json != null && Json.TryGetValue(name, out value))
            {
                long longValue = 0;
                if (long.TryParse(value.ToString(), out longValue))
                {
                    return longValue;
                }
            }

            return 0;
        }
    }
}