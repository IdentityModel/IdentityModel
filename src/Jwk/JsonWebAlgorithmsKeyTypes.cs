// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Jwk;

/// <summary>
/// Constants for JsonWebAlgorithms  "kty" Key Type (sec 6.1)
/// http://tools.ietf.org/html/rfc7518#section-6.1
/// </summary>
public static class JsonWebAlgorithmsKeyTypes
{
#pragma warning disable 1591
    public const string EllipticCurve = "EC";
    public const string RSA = "RSA";
    public const string Octet = "oct";
#pragma warning restore 1591
}