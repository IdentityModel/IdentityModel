// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.HttpClientExtensions
{
    public enum ClientCredentialStyle
    {
        /// <summary>
        /// HTTP basic authentication
        /// </summary>
        AuthorizationHeader,

        /// <summary>
        /// Post values in body
        /// </summary>
        PostBody,

        /// <summary>
        /// Put credentials on query string
        /// </summary>
        QueryString,
    };
}