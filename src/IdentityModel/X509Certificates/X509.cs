// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Security.Cryptography.X509Certificates;

namespace IdentityModel
{
    public static class X509
    {
        public static X509CertificatesLocation CurrentUser 
        { 
            get
            {
                return new X509CertificatesLocation(StoreLocation.CurrentUser);
            }
        }

        public static X509CertificatesLocation LocalMachine
        {
            get
            {
                return new X509CertificatesLocation(StoreLocation.LocalMachine);
            }
        }
    }
}
