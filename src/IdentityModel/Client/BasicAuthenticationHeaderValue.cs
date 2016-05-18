// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
    public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        public BasicAuthenticationHeaderValue(string userName, string password)
            : base("Basic", EncodeCredential(userName, password))
        { }

        private static string EncodeCredential(string userName, string password)
        {
            Encoding encoding = Encoding.UTF8;
            string credential = String.Format("{0}:{1}", userName, password);

            return Convert.ToBase64String(encoding.GetBytes(credential));
        }
    }
}