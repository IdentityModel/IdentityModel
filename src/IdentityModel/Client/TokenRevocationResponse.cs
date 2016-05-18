// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace IdentityModel.Client
{
    public class TokenRevocationResponse
    {
        public string Raw { get; protected set; }
        public JObject Json { get; protected set; }

        private bool _isHttpError;
        private HttpStatusCode _httpErrorstatusCode;
        private string _httpErrorReason;

        public TokenRevocationResponse()
        {
        }

        public TokenRevocationResponse(string raw)
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

        public TokenRevocationResponse(HttpStatusCode statusCode, string reason)
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

        public string Error
        {
            get
            {
                return GetStringOrNull(OidcConstants.TokenResponse.Error);
            }
        }

        public string ErrorDescription
        {
            get
            {
                return GetStringOrNull(OidcConstants.TokenResponse.ErrorDescription);
            }
        }

        public bool IsError
        {
            get
            {
                return (IsHttpError ||
                        !string.IsNullOrWhiteSpace(GetStringOrNull(OidcConstants.TokenResponse.Error)));
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
    }
}