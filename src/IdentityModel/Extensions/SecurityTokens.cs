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

#if NET451

using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace IdentityModel.Extensions
{
    /// <summary>
    /// Extension methods to convert tokens to string and claims
    /// </summary>
    public static class SecurityTokensExtensions
    {
        #region GenericXmlSecurityToken Special Cases
        /// <summary>
        /// Turns a supported generic XML security token to a security token.
        /// </summary>
        /// <param name="token">The generic XML security token.</param>
        /// <returns>A SecurityToken</returns>
        public static SecurityToken ToSecurityToken(this GenericXmlSecurityToken token)
        {
            var handler = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            return token.ToSecurityToken(handler);
        }

        /// <summary>
        /// Turns a supported generic XML security token to a security token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="decryptionCertificate">The decryption certificate.</param>
        /// <returns>A SecurityToken</returns>
        public static SecurityToken ToSecurityToken(this GenericXmlSecurityToken token, X509Certificate2 decryptionCertificate)
        {
            var configuration = new SecurityTokenHandlerConfiguration();
            configuration.ServiceTokenResolver = decryptionCertificate.CreateSecurityTokenResolver();

            var handler = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(configuration);
            return token.ToSecurityToken(handler);
        }

        /// <summary>
        /// Turns a supported generic XML security token to a security token.
        /// </summary>
        /// <param name="token">The generic XML security token.</param>
        /// <param name="handler">The security token handler.</param>
        /// <returns>A SecurityToken</returns>
        public static SecurityToken ToSecurityToken(this GenericXmlSecurityToken token, SecurityTokenHandlerCollection handler)
        {
            var reader = new XmlTextReader(new StringReader(token.TokenXml.OuterXml));

            if (handler.CanReadToken(reader))
            {
                return handler.ReadToken(reader);
            }
            else
            {
                throw new InvalidOperationException("Unsupported token type");
            }
        }

        /// <summary>
        /// Retrieves the XML from a GenericXmlSecurityToken
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The token XML string.</returns>
        public static string ToTokenXmlString(this GenericXmlSecurityToken token)
        {
            return token.TokenXml.OuterXml;
        }
        #endregion

        #region To Token XML String
        /// <summary>
        /// Converts a supported token to an XML string.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The token XML string.</returns>
        public static string ToTokenXmlString(this SecurityToken token)
        {
            var genericToken = token as GenericXmlSecurityToken;
            if (genericToken != null)
            {
                return genericToken.ToTokenXmlString();
            }

            var handler = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            return token.ToTokenXmlString(handler);
        }

        /// <summary>
        /// Converts a supported token to an XML string.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="handler">The token handler.</param>
        /// <returns>The token XML string.</returns>
        public static string ToTokenXmlString(this SecurityToken token, SecurityTokenHandlerCollection handler)
        {
            if (handler.CanWriteToken(token))
            {
                var sb = new StringBuilder(128);
                handler.WriteToken(new XmlTextWriter(new StringWriter(sb)), token);
                return sb.ToString();
            }
            else
            {
                throw new InvalidOperationException("Token type not suppoted");
            }
        }
        #endregion

        #region To ClaimsPrincipal
        /// <summary>
        /// Converts a SecurityToken to an IClaimsPrincipal.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="signingCertificate">The signing certificate.</param>
        /// <returns>An IClaimsPrincipal</returns>
        public static ClaimsPrincipal ToClaimsPrincipal(this SecurityToken token, X509Certificate2 signingCertificate)
        {
            var configuration = CreateStandardConfiguration(signingCertificate);
            return token.ToClaimsPrincipal(configuration.CreateDefaultHandlerCollection());
        }

        /// <summary>
        /// Converts a SecurityToken to an IClaimsPrincipal.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="signingCertificate">The signing certificate.</param>
        /// <param name="audienceUri">The audience URI.</param>
        /// <returns>An IClaimsPrincipal</returns>
        public static ClaimsPrincipal ToClaimsPrincipal(this SecurityToken token, X509Certificate2 signingCertificate, string audienceUri)
        {
            var configuration = CreateStandardConfiguration(signingCertificate);

            configuration.AudienceRestriction.AudienceMode = AudienceUriMode.Always;
            configuration.AudienceRestriction.AllowedAudienceUris.Add(new Uri(audienceUri));

            return token.ToClaimsPrincipal(configuration.CreateDefaultHandlerCollection());
        }

        /// <summary>
        /// Converts a SecurityToken to an IClaimsPrincipal.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>An IClaimsPrincipal</returns>
        public static ClaimsPrincipal ToClaimsPrincipal(this SecurityToken token, SecurityTokenHandlerCollection handler)
        {
            var ids = handler.ValidateToken(token);

            return new ClaimsPrincipal(from identity in ids select identity);
        }

        private static SecurityTokenHandlerConfiguration CreateStandardConfiguration(X509Certificate2 signingCertificate)
        {
            var configuration = new SecurityTokenHandlerConfiguration();

            configuration.AudienceRestriction.AudienceMode = AudienceUriMode.Never;
            configuration.IssuerNameRegistry = signingCertificate.CreateIssuerNameRegistry();
            configuration.IssuerTokenResolver = signingCertificate.CreateSecurityTokenResolver();
            configuration.SaveBootstrapContext = true;

            return configuration;
        }

        private static IssuerNameRegistry CreateIssuerNameRegistry(this X509Certificate2 certificate)
        {
            var registry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer(certificate.Thumbprint, certificate.Subject);

            return registry;
        }

        private static SecurityTokenResolver CreateSecurityTokenResolver(this X509Certificate2 certificate)
        {
            var tokens = new List<SecurityToken>
            {
                new X509SecurityToken(certificate)
            };

            return SecurityTokenResolver.CreateDefaultSecurityTokenResolver(tokens.AsReadOnly(), true);
        }

        private static SecurityTokenHandlerCollection CreateDefaultHandlerCollection(this SecurityTokenHandlerConfiguration configuration)
        {
            return SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(configuration);
        }
        #endregion
    }
}

#endif