// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.



namespace Windows.Web.Http.Filters
{
    public static class HttpClientExtensions
    {
        public static void SetBasicAuthentication(this HttpClient client, string userName, string password)
        {
            client.DefaultRequestHeaders.Authorization = HttpCredentialsHeaderValueExtensions.CreateBasic(userName, password);
        }

        public static void SetToken(this HttpClient client, string scheme, string token)
        {
            client.DefaultRequestHeaders.Authorization = HttpCredentialsHeaderValueExtensions.CreateBasic(scheme, token);
        }

        public static void SetBearerToken(this HttpClient client, string token)
        {
            client.SetToken("Bearer", token);
        }
    }
}
