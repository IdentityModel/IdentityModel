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

        public bool IsHttpError { get; }
        public ushort HttpErrorStatusCode { get; }
        public string HttpErrorReason { get; }

        public string Error => GetStringOrNull(OidcConstants.TokenResponse.Error);
        public string ErrorDescription => GetStringOrNull(OidcConstants.TokenResponse.ErrorDescription);
        public bool IsError
            => (IsHttpError ||
               !string.IsNullOrWhiteSpace(GetStringOrNull(OidcConstants.TokenResponse.Error)));

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

        public TokenRevocationResponse(ushort statusCode, string reason)
        {
            IsHttpError = true;
            HttpErrorStatusCode = statusCode;
            HttpErrorReason = reason;
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