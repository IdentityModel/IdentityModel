// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Newtonsoft.Json.Linq;
using System;
using System.Net;
using IdentityModel.Client;

namespace System.Net.Http
{
    public static class HttpStatusCodeExtensions
    {
        public static HttpStatusCode GetHttpErrorStatusCode(this TokenRevocationResponse response)
        {
            return (HttpStatusCode)response.HttpErrorStatusCode;
        }

        public static HttpStatusCode GetHttpErrorStatusCode(this UserInfoResponse response)
        {
            return (HttpStatusCode)response.HttpStatusCode;
        }
    }
}