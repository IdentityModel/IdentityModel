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

namespace IdentityModel.Constants
{
    public static class SamlNameIdentifierFormats
    {
        public const string EmailAddressString = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";
        public const string EncryptedString = "urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted";
        public const string EntityString = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";
        public const string KerberosString = "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos";
        public const string PersistentString = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";
        public const string TransientString = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";
        public const string UnspecifiedString = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
        public const string WindowsDomainQualifiedNameString = "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName";
        public const string X509SubjectNameString = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
    }
}