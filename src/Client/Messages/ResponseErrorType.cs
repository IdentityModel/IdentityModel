// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client;

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
    /// protocol related - valid response, but some protocol level error.
    /// </summary>
    Protocol,

    /// <summary>
    /// HTTP error - e.g. 404.
    /// </summary>
    Http,

    /// <summary>
    /// An exception occurred - exception while connecting to the endpoint, e.g. TLS problems.
    /// </summary>
    Exception,

    /// <summary>
    /// A policy violation - a configured policy was violated.
    /// </summary>
    PolicyViolation
}