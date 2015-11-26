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
