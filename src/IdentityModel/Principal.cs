// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace IdentityModel
{
    /// <summary>
    /// Helper class to create ClaimsPrincipal
    /// </summary>
    public static class Principal
    {
        /// <summary>
        /// Gets an anoymous ClaimsPrincipal.
        /// </summary>
        public static ClaimsPrincipal Anonymous => new ClaimsPrincipal(Identity.Anonymous);

        /// <summary>
        /// Creates a ClaimsPrincipal using the specified authentication type and claims.
        /// </summary>
        /// <param name="authenticationType">Type of the authentication.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public static ClaimsPrincipal Create(string authenticationType, params Claim[] claims)
        {
            return new ClaimsPrincipal(Identity.Create(authenticationType, claims));
        }

        /// <summary>
        /// Creates a ClaimsPrincipal based on information found in an X509 certificate.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <param name="authenticationType">Type of the authentication.</param>
        /// <param name="includeAllClaims">if set to <c>true</c> [include all claims].</param>
        /// <returns></returns>
        public static ClaimsPrincipal CreateFromCertificate(X509Certificate2 certificate, string authenticationType = "X.509", bool includeAllClaims = false)
        {
            return new ClaimsPrincipal(Identity.CreateFromCertificate(certificate, authenticationType, includeAllClaims));
        }
    }
}