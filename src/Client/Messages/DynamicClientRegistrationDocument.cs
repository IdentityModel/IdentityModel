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
    /// Models an OpenID Connect dynamic client registration request
    /// </summary>
    public class DynamicClientRegistrationDocument
    {
        [JsonPropertyName(OidcConstants.ClientMetadata.RedirectUris)]
        public ICollection<string> RedirectUris { get; set; } = new HashSet<string>();

        [JsonPropertyName(OidcConstants.ClientMetadata.ResponseTypes)]
        public ICollection<string> ResponseTypes { get; set; } = new HashSet<string>();

        [JsonPropertyName(OidcConstants.ClientMetadata.GrantTypes)]
        public ICollection<string> GrantTypes { get; set; } = new HashSet<string>();

        [JsonPropertyName(OidcConstants.ClientMetadata.ApplicationType)]
        public string ApplicationType { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.Contacts)]
        public ICollection<string> Contacts { get; set; } = new HashSet<string>();

        [JsonPropertyName(OidcConstants.ClientMetadata.ClientName)]
        public string ClientName { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.LogoUri)]
        public string LogoUri { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.ClientUri)]
        public string ClientUri { get; set; }

        [JsonPropertyName(OidcConstants.ClientMetadata.PolicyUri)]
        public string PolicyUri { get; set; }

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

        // don't serialize empty arrays
        public bool ShouldSerializeRequestUris()
        {
            return RequestUris.Any();
        }

        public bool ShouldSerializeDefaultAcrValues()
        {
            return DefaultAcrValues.Any();
        }

        public bool ShouldSerializeResponseTypes()
        {
            return ResponseTypes.Any();
        }

        public bool ShouldSerializeGrantTypes()
        {
            return GrantTypes.Any();
        }

        public bool ShouldSerializeContacts()
        {
            return Contacts.Any();
        }
    }
}