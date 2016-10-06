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
        public HttpStatusCode HttpStatusCode { get; }
        public string HttpErrorReason { get; }
        public ResponseErrorType ErrorType { get;  }
        public Exception Exception { get;  }

        public TokenRevocationResponse()
        {
            IsError = false;
        }

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
                IsError = true;
                ErrorType = ResponseErrorType.Exception;
                Exception = ex;
            }
        }

        public TokenRevocationResponse(HttpStatusCode statusCode, string reason)
        {
            IsError = true;

            ErrorType = ResponseErrorType.Http;
            HttpStatusCode = statusCode;
            HttpErrorReason = reason;
        }

        public TokenRevocationResponse(Exception exception)
        {
            IsError = true;

            Exception = exception;
            ErrorType = ResponseErrorType.Exception;
        }

        public string Error
        {
            get
            {
                if (ErrorType == ResponseErrorType.Http)
                {
                    return HttpErrorReason;
                }
                else if(ErrorType == ResponseErrorType.Exception)
                {
                    return Exception.Message;
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