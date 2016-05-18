// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace IdentityModel
{
    public static class Principal
    {
        public static ClaimsPrincipal Anonymous
        {
            get
            {
                return new ClaimsPrincipal(Identity.Anonymous);
            }
        }

        public static ClaimsPrincipal Create(string authenticationType, params Claim[] claims)
        {
            return new ClaimsPrincipal(Identity.Create(authenticationType, claims));
        }

        public static ClaimsPrincipal CreateFromCertificate(X509Certificate2 certificate, string authenticationType = "X.509", bool includeAllClaims = false)
        {
            return new ClaimsPrincipal(Identity.CreateFromCertificate(certificate, authenticationType, includeAllClaims));
        }
    }
}