// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Jwk;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#pragma warning disable 1591

namespace IdentityModel.Client
{
    /// <summary>
    /// Models an OpenID Connect dynamic client registration request.
    /// </summary>
    /// <remarks>
    /// <para>This class gets serialized. It may be inherited in order to extend it with custom properties.</para>
    /// <para>https://datatracker.ietf.org/doc/html/rfc7591</para>
    /// </remarks>
    public class DynamicClientRegistrationDocument
    {
        [JsonPropertyName(OidcConstants.ClientMetadata.RedirectUris)]
        public ICollection<string> RedirectUris { get; set; } = new HashSet<string>();

        /// <summary>
        /// List of the OAuth 2.0 response type strings that the client can use at the authorization endpoint.
        /// </summary>
        [JsonPropertyName(OidcConstants.ClientMetadata.ResponseTypes)]
        public ICollection<string> ResponseTypes { get; set; } = new HashSet<string>();

        [JsonPropertyName(OidcConstants.ClientMetadata.GrantTypes)]
        public ICollection<string> GrantTypes { get; set; } = new HashSet<string>();

        [JsonPropertyName(OidcConstants.ClientMetadata.ApplicationType)]
        public string ApplicationType { get; set; }

        /// <summary>
        /// List of strings representing ways to contact people responsible for this client, typically email addresses.
        /// </summary>
        [JsonPropertyName(OidcConstants.ClientMetadata.Contacts)]
        public ICollection<string> Contacts { get; set; } = new HashSet<string>();

        /// <summary>
        /// Human-readable string name of the client to be presented to the end-user during authorization.
        /// </summary>
        [JsonPropertyName(OidcConstants.ClientMetadata.ClientName)]
        public string ClientName { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.LogoUri)]
        public string LogoUri { get; set; }

        /// <summary>
        /// Web page providing information about the client.
        /// </summary>
        [JsonPropertyName(OidcConstants.ClientMetadata.ClientUri)]
        public string ClientUri { get; set; }

        /// <summary>
        /// Human-readable privacy policy document that describes how the deployment organization
        /// collects, uses, retains, and discloses personal data.
        /// </summary>
        [JsonPropertyName(OidcConstants.ClientMetadata.PolicyUri)]
        public string PolicyUri { get; set; }

        /// <summary>
        /// Human-readable terms of service document for the client that describes a contractual relationship
        /// between the end-user and the client that the end-user accepts when authorizing the client.
        /// </summary>
        [JsonPropertyName(OidcConstants.ClientMetadata.TosUri)]
        public string TosUri { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.JwksUri)]
        public string JwksUri { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.Jwks)]
        public JsonWebKeySet Jwks { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.SectorIdentifierUri)]
        public string SectorIdentifierUri { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.SubjectType)]
        public string SubjectType { get; set; }

        /// <summary>
        /// String containing a space-separated list of scope values that the client can use when requesting access tokens.
        /// </summary>
        [JsonPropertyName(OidcConstants.ClientMetadata.Scope)]
        public string Scope { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenSignedResponseAlgorithm)]
        public string IdentityTokenSignedResponseAlgorithm { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenEncryptedResponseAlgorithm)]
        public string IdentityTokenEncryptedResponseAlgorithm { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenEncryptedResponseEncryption)]
        public string IdentityTokenEncryptedResponseEncryption { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.UserinfoSignedResponseAlgorithm)]
        public string UserinfoSignedResponseAlgorithm { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.UserInfoEncryptedResponseAlgorithm)]
        public string UserInfoEncryptedResponseAlgorithm { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.UserinfoEncryptedResponseEncryption)]
        public string UserinfoEncryptedResponseEncryption { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectSigningAlgorithm)]
        public string RequestObjectSigningAlgorithm { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectEncryptionAlgorithm)]
        public string RequestObjectEncryptionAlgorithm { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectEncryptionEncryption)]
        public string RequestObjectEncryptionEncryption { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.TokenEndpointAuthenticationMethod)]
        public string TokenEndpointAuthenticationMethod { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.TokenEndpointAuthenticationSigningAlgorithm)]
        public string TokenEndpointAuthenticationSigningAlgorithm { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.DefaultMaxAge)]
        public int DefaultMaxAge { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.RequireAuthenticationTime)]
        public bool RequireAuthenticationTime { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.DefaultAcrValues)]
        public ICollection<string> DefaultAcrValues { get; set; } = new HashSet<string>();

        [JsonPropertyName(OidcConstants.ClientMetadata.InitiateLoginUris)]
        public string InitiateLoginUri { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.RequestUris)]
        public ICollection<string> RequestUris { get; set; } = new HashSet<string>();

        // Don't serialize empty arrays
        public bool ShouldSerializeRequestUris() => RequestUris.Any();

        public bool ShouldSerializeDefaultAcrValues() => DefaultAcrValues.Any();

        public bool ShouldSerializeResponseTypes() => ResponseTypes.Any();

        public bool ShouldSerializeGrantTypes() => GrantTypes.Any();

        public bool ShouldSerializeContacts() => Contacts.Any();
    }
}
