// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client
{
    /// <summary>
    /// Various reasons for a protocol endpoint error
    /// </summary>
    public enum ResponseErrorType
    {
        /// <summary>
        /// none
        /// </summary>
        None,

        /// <summary>
        /// protocol related
        /// </summary>
        Protocol,

        /// <summary>
        /// HTTP error
        /// </summary>
        Http,

        /// <summary>
        /// An exception occurred
        /// </summary>
        Exception,

        /// <summary>
        /// A policy violation
        /// </summary>
        PolicyViolation
    }
}