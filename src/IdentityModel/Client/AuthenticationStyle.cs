// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client
{
    /// <summary>
    /// Enum for specifying the authentication style of a client
    /// </summary>
    public enum AuthenticationStyle
    {
        /// <summary>
        /// HTTP basic authentication
        /// </summary>
        BasicAuthentication,

        /// <summary>
        /// post values in body
        /// </summary>
        PostValues,

        /// <summary>
        /// custom
        /// </summary>
        Custom
    };
}