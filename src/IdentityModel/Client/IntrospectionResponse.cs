// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace IdentityModel.Client
{
    public class IntrospectionResponse
    {
        public string Raw { get; }
        public JObject Json { get; }

        public bool IsError { get; }
        public string Error { get; }
        public ResponseErrorType ErrorType { get; } = ResponseErrorType.None;
        public Exception Exception { get; }
        public HttpStatusCode HttpStatusCode { get; }

        public bool IsActive { get; }
        public IEnumerable<Claim> Claims { get; }

        public IntrospectionResponse(string raw)
        {
            Raw = raw;

            try
            {
                Json = JObject.Parse(raw);
                IsActive = bool.Parse(Json["active"].ToString());
                Claims = Json.ToClaims();
            }
            catch (Exception ex)
            {
                IsError = true;
                Error = ex.ToString();
            }
        }

        public IntrospectionResponse(Exception exception)
        {
            IsError = true;

            Exception = exception;
            ErrorType = ResponseErrorType.Exception;
        }

        public IntrospectionResponse(HttpStatusCode statusCode, string reason)
        {
            IsError = true;

            ErrorType = ResponseErrorType.Http;
            HttpStatusCode = statusCode;
            Error = reason;
        }
    }
}