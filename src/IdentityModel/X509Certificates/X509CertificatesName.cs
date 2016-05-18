// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace IdentityModel
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class X509CertificatesName
    {
        StoreLocation _location;
        StoreName _name;

        public X509CertificatesName(StoreLocation location, StoreName name)
        {
            _location = location;
            _name = name;
        }

        public X509CertificatesFinder Thumbprint
        {
            get
            {
                return new X509CertificatesFinder(_location, _name, X509FindType.FindByThumbprint);
            }
        }

        public X509CertificatesFinder SubjectDistinguishedName
        {
            get
            {
                return new X509CertificatesFinder(_location, _name, X509FindType.FindBySubjectDistinguishedName);
            }
        }

        public X509CertificatesFinder SerialNumber
        {
            get
            {
                return new X509CertificatesFinder(_location, _name, X509FindType.FindBySerialNumber);
            }
        }

        public X509CertificatesFinder IssuerName
        {
            get
            {
                return new X509CertificatesFinder(_location, _name, X509FindType.FindByIssuerName);
            }
        }
    }
}
