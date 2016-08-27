// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Text;
using Windows.Web.Http.Headers;

namespace Windows.Web.Http.Filters
{
    public static class HttpCredentialsHeaderValueExtensions
    {
        public static HttpCredentialsHeaderValue CreateBasic(string userName, string password)
        {
            return new HttpCredentialsHeaderValue("Basic", EncodeCredential(userName, password));
        }

        private static string EncodeCredential(string userName, string password)
        {
            Encoding encoding = Encoding.UTF8;
            string credential = string.Format("{0}:{1}", userName, password);

            return Convert.ToBase64String(encoding.GetBytes(credential));
        }
    }
}