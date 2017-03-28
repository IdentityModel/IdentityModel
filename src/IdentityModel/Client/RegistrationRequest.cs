// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Jwk;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 1591

namespace IdentityModel.Client
{
    /// <summary>
    /// Models an OpenID Connect dynamic client registration request
    /// </summary>
    public class RegistrationRequest
    {
        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.RedirectUris, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public ICollection<string> RedirectUris { get; set; } = new HashSet<string>();

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.ResponseTypes, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public ICollection<string> ResponseTypes { get; set; } = new HashSet<string>();

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.GrantTypes, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public ICollection<string> GrantTypes { get; set; } = new HashSet<string>();

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.ApplicationType, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string ApplicationType { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.Contacts, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public ICollection<string> Contacts { get; set; } = new HashSet<string>();

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.ClientName, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string ClientName { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.LogoUri, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string LogoUri { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.ClientUri, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string ClientUri { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.PolicyUri, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string PolicyUri { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.TosUri, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string TosUri { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.JwksUri, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string JwksUri { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.Jwks, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public JsonWebKeySet Jwks { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.SectorIdentifierUri, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string SectorIdentifierUri { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.SubjectType, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string SubjectType { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.IdentityTokenSignedResponseAlgorithm, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string IdentityTokenSignedResponseAlgorithm { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.IdentityTokenEncryptedResponseAlgorithm, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string IdentityTokenEncryptedResponseAlgorithm { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.IdentityTokenEncryptedResponseEncryption, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string IdentityTokenEncryptedResponseEncryption { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.UserinfoSignedResponseAlgorithm, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string UserinfoSignedResponseAlgorithm { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.UserInfoEncryptedResponseAlgorithm, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string UserInfoEncryptedResponseAlgorithm { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.UserinfoEncryptedResponseEncryption, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string UserinfoEncryptedResponseEncryption { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.RequestObjectSigningAlgorithm, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string RequestObjectSigningAlgorithm { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.RequestObjectEncryptionAlgorithm, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string RequestObjectEncryptionAlgorithm { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.RequestObjectEncryptionEncryption, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string RequestObjectEncryptionEncryption { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.TokenEndpointAuthenticationMethod, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string TokenEndpointAuthenticationMethod { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.TokenEndpointAuthenticationSigningAlgorithm, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string TokenEndpointAuthenticationSigningAlgorithm { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.DefaultMaxAge, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public int DefaultMaxAge { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.RequireAuthenticationTime, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public bool RequireAuthenticationTime { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.DefaultAcrValues, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public ICollection<string> DefaultAcrValues { get; set; } = new HashSet<string>();

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.InitiateLoginUris, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string InitiateLoginUri { get; set; }

        [JsonProperty(PropertyName = OidcConstants.ClientMetadata.RequestUris, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
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