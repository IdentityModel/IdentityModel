// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

#pragma warning disable 1591

namespace IdentityModel
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class X509CertificatesFinder
    {
        readonly StoreLocation _location;
        readonly StoreName _name;
        readonly X509FindType _findType;

        public X509CertificatesFinder(StoreLocation location, StoreName name, X509FindType findType)
        {
            _location = location;
            _name = name;
            _findType = findType;
        }

        public IEnumerable<X509Certificate2> Find(object findValue, bool validOnly = true)
        {
#if NET452
            var store = new X509Store(_name, _location);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var certColl = store.Certificates.Find(_findType, findValue, validOnly);
                store.Close();
                return certColl.Cast<X509Certificate2>();
            }
            finally
            {
                store.Close();
            }
#else
            using (var store = new X509Store(_name, _location))
            {
                store.Open(OpenFlags.ReadOnly);

                var certColl = store.Certificates.Find(_findType, findValue, validOnly);
                return certColl.Cast<X509Certificate2>();
            }
#endif
        }
    }
}