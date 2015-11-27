/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace IdentityModel
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class X509CertificatesLocation
    {
        StoreLocation _location;

        public X509CertificatesLocation(StoreLocation location)
        {
            _location = location;
        }

        public X509CertificatesName My
        {
            get
            {
                return new X509CertificatesName(_location, StoreName.My);
            }
        }

        public X509CertificatesName AddressBook
        {
            get
            {
                return new X509CertificatesName(_location, StoreName.AddressBook);
            }
        }

        public X509CertificatesName TrustedPeople
        {
            get
            {
                return new X509CertificatesName(_location, StoreName.TrustedPeople);
            }
        }

        public X509CertificatesName TrustedPublisher
        {
            get
            {
                return new X509CertificatesName(_location, StoreName.TrustedPublisher);
            }
        }

        public X509CertificatesName CertificateAuthority
        {
            get
            {
                return new X509CertificatesName(_location, StoreName.CertificateAuthority);
            }
        }
    }
}
