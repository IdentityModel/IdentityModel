// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace IdentityModel.Client
{
    public class UserInfoResponse
    {
        public string Raw { get; }
        public JObject Json { get; }
        public IEnumerable<Claim> Claims { get; }

        public bool IsHttpError { get; }
        public HttpStatusCode HttpStatusCode { get; }
        public string HttpErrorReason { get; }

        public bool IsError { get; }
        public string Error { get; }

        public UserInfoResponse(string raw)
        {
            Raw = raw;
            HttpStatusCode = HttpStatusCode.OK;

            try
            {
                Json = JObject.Parse(raw);
                Claims = Json.ToClaims();
            }
            catch (Exception ex)
            {
                IsError = true;
                Error = ex.Message;
            }
        }

        public UserInfoResponse(HttpStatusCode statusCode, string httpErrorReason)
        {
            IsError = true;

            HttpStatusCode = statusCode;
            Error = httpErrorReason;
        }
    }
}