// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

#pragma warning disable 1591

namespace IdentityModel
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class X509CertificatesLocation
    {
        readonly StoreLocation _location;

        public X509CertificatesLocation(StoreLocation location)
        {
            _location = location;
        }

        public X509CertificatesName My => new X509CertificatesName(_location, StoreName.My);
        public X509CertificatesName AddressBook => new X509CertificatesName(_location, StoreName.AddressBook);
        public X509CertificatesName TrustedPeople => new X509CertificatesName(_location, StoreName.TrustedPeople);
        public X509CertificatesName TrustedPublisher => new X509CertificatesName(_location, StoreName.TrustedPublisher);
        public X509CertificatesName CertificateAuthority => new X509CertificatesName(_location, StoreName.CertificateAuthority);
    }
}