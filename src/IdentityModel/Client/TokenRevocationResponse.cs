// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace IdentityModel.Client
{
    public class TokenRevocationResponse
    {
        public string Raw { get; }
        public JObject Json { get; }
        public bool IsError { get; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string HttpErrorReason { get; set; }
        public ResponseErrorType ErrorType { get; set; }

        public TokenRevocationResponse(string raw)
        {
            Raw = raw;
            IsError = false;
            HttpStatusCode = HttpStatusCode.OK;

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
            IsError = true;

            ErrorType = ResponseErrorType.Http;
            HttpStatusCode = statusCode;
            HttpErrorReason = reason;
        }

        public string Error
        {
            get
            {
                if (ErrorType == ResponseErrorType.Http)
                {
                    return HttpErrorReason;
                }

                return GetStringOrNull(OidcConstants.TokenResponse.Error);
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