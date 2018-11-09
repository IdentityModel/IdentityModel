// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Net.Http;
using System.Net.Http.Headers;

namespace IdentityModel.Client
{
    /// <summary>
    /// Extensions for HttpRequestMessage
    /// </summary>
    public static class AuthorizationHeaderExtensions
    {
        /// <summary>
        /// Sets a basic authentication header.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public static void SetBasicAuthentication(this HttpClient client, string userName, string password)
        {
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userName, password);
        }

        /// <summary>
        /// Sets a basic authentication header for RFC6749 client authentication.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public static void SetBasicAuthenticationOAuth(this HttpClient client, string userName, string password)
        {
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationOAuthHeaderValue(userName, password);
        }

        /// <summary>
        /// Sets an authorization header with a given scheme and value.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="token">The token.</param>
        public static void SetToken(this HttpClient client, string scheme, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }

        /// <summary>
        /// Sets an authorization header with a bearer token.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="token">The token.</param>
        public static void SetBearerToken(this HttpClient client, string token)
        {
            client.SetToken("Bearer", token);
        }

        /// <summary>
        /// Sets a basic authentication header.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public static void SetBasicAuthentication(this HttpRequestMessage request, string userName, string password)
        {
            request.Headers.Authorization = new BasicAuthenticationHeaderValue(userName, password);
        }
        
        /// <summary>
        /// Sets a basic authentication header for RFC6749 client authentication.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public static void SetBasicAuthenticationOAuth(this HttpRequestMessage request, string userName, string password)
        {
            request.Headers.Authorization = new BasicAuthenticationOAuthHeaderValue(userName, password);
        }

        /// <summary>
        /// Sets an authorization header with a given scheme and value.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="token">The token.</param>
        public static void SetToken(this HttpRequestMessage request, string scheme, string token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);
        }

        /// <summary>
        /// Sets an authorization header with a bearer token.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="token">The token.</param>
        public static void SetBearerToken(this HttpRequestMessage request, string token)
        {
            request.SetToken("Bearer", token);
        }
    }
}