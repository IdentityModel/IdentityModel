// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Jwk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#pragma warning disable 1591

namespace IdentityModel.Client;

/// <summary>
/// Models an OpenID Connect dynamic client registration request.
/// </summary>
/// <remarks>
/// <see href="https://datatracker.ietf.org/doc/html/rfc7591" /> and <see href="https://openid.net/specs/openid-connect-registration-1_0.html" />.
/// </remarks>
public class DynamicClientRegistrationDocument
{
    /// <summary>
    /// List of redirection URI strings for use in redirect-based flows such as the authorization code and implicit flows.
    /// </summary>
    /// <remarks>
    /// Clients using flows with redirection must register their redirection URI values.
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.RedirectUris)]
    public ICollection<Uri> RedirectUris { get; set; } = new HashSet<Uri>();

    /// <summary>
    /// List of the OAuth 2.0 response type strings that the client can use at the authorization endpoint.
    /// </summary>
    /// <remarks>
    /// Example: "code" or "token".
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.ResponseTypes)]
    public ICollection<string> ResponseTypes { get; set; } = new HashSet<string>();

    /// <summary>
    /// List of OAuth 2.0 grant type strings that the client can use at the token endpoint.
    /// </summary>
    /// <remarks>
    /// Example: "authorization_code", "implicit", "password", "client_credentials", "refresh_token".
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.GrantTypes)]
    public ICollection<string> GrantTypes { get; set; } = new HashSet<string>();

    /// <summary>
    /// Kind of the application.
    /// </summary>
    /// <remarks>
    /// The defined values are "native" or "web".
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.ApplicationType)]
    public string? ApplicationType { get; set; }

    /// <summary>
    /// List of strings representing ways to contact people responsible for this client, typically email addresses.
    /// </summary>
    /// <remarks>
    /// The authorization server may make these contact addresses available to end-users for support requests for the client.
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.Contacts)]
    public ICollection<string> Contacts { get; set; } = new HashSet<string>();

    /// <summary>
    /// Human-readable string name of the client to be presented to the end-user during authorization.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.ClientName)]
    public string? ClientName { get; set; }

    /// <summary>
    /// Logo for the client.
    /// </summary>
    /// <remarks>
    /// If present, the server should display this image to the end-user during approval.
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.LogoUri)]
    public Uri? LogoUri { get; set; }

    /// <summary>
    /// Web page providing information about the client.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.ClientUri)]
    public Uri? ClientUri { get; set; }

    /// <summary>
    /// Human-readable privacy policy document that describes how the deployment organization
    /// collects, uses, retains, and discloses personal data.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.PolicyUri)]
    public Uri? PolicyUri { get; set; }

    /// <summary>
    /// Human-readable terms of service document for the client that describes a contractual relationship
    /// between the end-user and the client that the end-user accepts when authorizing the client.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.TosUri)]
    public Uri? TosUri { get; set; }

    /// <summary>
    /// JWK Set document which contains the client's public keys.
    /// </summary>
    /// <remarks>
    /// Use of this parameter is preferred over the "jwks" parameter, as it allows for easier key rotation.
    /// The <see cref="JwksUri"/> and <see cref="Jwks"/> parameters MUST NOT both be present in
    /// the same request or response.
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.JwksUri)]
    public Uri? JwksUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.Jwks)]
    public JsonWebKeySet? Jwks { get; set; }

    /// <summary>
    /// URL using the https scheme to be used in calculating Pseudonymous Identifiers by the OpenID provider.
    /// </summary>
    /// <remarks>
    /// The URL references a file with a single JSON array of <c>redirect_uri</c> values. 
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.SectorIdentifierUri)]
    public Uri? SectorIdentifierUri { get; set; }

    /// <remarks>
    /// Valid types include "pairwise" and "public".
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.SubjectType)]
    public string? SubjectType { get; set; }

    /// <summary>
    /// String containing a space-separated list of scope values that the client can use when requesting access tokens.
    /// </summary>
    /// <remarks>
    /// If omitted, an authorization server may register a client with a default set of scopes.
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.Scope)]
    public string? Scope { get; set; }

    /// <summary>
    /// List of post-logout redirection URIs for use in the end session
    /// endpoint.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.PostLogoutRedirectUris)]
    public ICollection<Uri> PostLogoutRedirectUris { get; set; } = new HashSet<Uri>();

    /// <summary>
    /// RP URL that will cause the RP to log itself out when rendered in an
    /// iframe by the OP.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.FrontChannelLogoutUri)]
    public string? FrontChannelLogoutUri { get; set; }

    /// <summary>
    /// Boolean value specifying whether the RP requires that a sid (session ID)
    /// query parameter be included to identify the RP session with the OP when
    /// the frontchannel_logout_uri is used.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.FrontChannelLogoutSessionRequired)]
    public bool? FrontChannelLogoutSessionRequired { get; set; }

    /// <summary>
    /// RP URL that will cause the RP to log itself out when sent a Logout Token
    /// by the OP.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.BackchannelLogoutUri)]
    public string? BackChannelLogoutUri { get; set; }

    /// <summary>
    /// Boolean value specifying whether the RP requires that a sid (session ID)
    /// Claim be included in the Logout Token to identify the RP session with
    /// the OP when the backchannel_logout_uri is used.e
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.BackchannelLogoutSessionRequired)]
    public bool? BackchannelLogoutSessionRequired { get; set; }

    /// <summary>
    /// A software statement containing client metadata values about the client
    /// software as claims.  This is a string value containing the entire signed
    /// JWT.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.SoftwareStatement)]
    public string? SoftwareStatement { get; set; }

    /// <summary>
    /// A unique identifier string (e.g., a <see cref="System.Guid"/>) assigned by the client developer or software
    /// publisher used by registration endpoints to identify the client software to be dynamically registered.
    /// </summary>
    /// <remarks>
    /// The value of this field is not intended to be human readable and is usually opaque to the client and authorization server.
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.SoftwareId)]
    public string? SoftwareId { get; set; }

    /// <summary>
    /// A version identifier string for the client software identified by <see cref="SoftwareId"/>.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.SoftwareVersion)]
    public string? SoftwareVersion { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenSignedResponseAlgorithm)]
    public string? IdentityTokenSignedResponseAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenEncryptedResponseAlgorithm)]
    public string? IdentityTokenEncryptedResponseAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenEncryptedResponseEncryption)]
    public string? IdentityTokenEncryptedResponseEncryption { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.UserinfoSignedResponseAlgorithm)]
    public string? UserinfoSignedResponseAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.UserInfoEncryptedResponseAlgorithm)]
    public string? UserInfoEncryptedResponseAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.UserinfoEncryptedResponseEncryption)]
    public string? UserinfoEncryptedResponseEncryption { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectSigningAlgorithm)]
    public string? RequestObjectSigningAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectEncryptionAlgorithm)]
    public string? RequestObjectEncryptionAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectEncryptionEncryption)]
    public string? RequestObjectEncryptionEncryption { get; set; }

    /// <summary>
    /// Boolean value specifying whether authorization requests must be
    /// protected as signed request objects and provided through either the
    /// request or request_uri parameters.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.RequireSignedRequestObject)]
    public bool? RequireSignedRequestObject { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.TokenEndpointAuthenticationMethod)]
    public string? TokenEndpointAuthenticationMethod { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.TokenEndpointAuthenticationSigningAlgorithm)]
    public string? TokenEndpointAuthenticationSigningAlgorithm { get; set; }

    /// <summary>
    /// Default maximum authentication age.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.DefaultMaxAge)]
    public int? DefaultMaxAge { get; set; }

    /// <summary>
    /// Whether the <c>auth_time</c> claim in the id token is required.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.RequireAuthenticationTime)]
    public bool? RequireAuthenticationTime { get; set; }

    /// <summary>
    /// Default requested Authentication Context Class Reference values.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.DefaultAcrValues)]
    public ICollection<string> DefaultAcrValues { get; set; } = new HashSet<string>();

    /// <summary>
    /// URI using the https scheme that a third party can use to initiate a
    /// login by the relying party.
    /// </summary>
    /// <remarks>
    /// The URI must accept requests via both GET and POST. The client must
    /// understand the <c>login_hint</c> and iss parameters and should support
    /// the <c>target_link_uri</c> parameter.
    /// </remarks>
    [JsonPropertyName(OidcConstants.ClientMetadata.InitiateLoginUri)]
    public Uri? InitiateLoginUri { get; set; }

    /// <summary>
    /// List of request URI values that are pre-registered by the relying party for use at the OpenID provider.
    /// </summary>
    [JsonPropertyName(OidcConstants.ClientMetadata.RequestUris)]
    public ICollection<Uri> RequestUris { get; set; } = new HashSet<Uri>();
    
    /// <summary>
    /// Custom client metadata fields to include in the serialization.
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object>? Extensions { get; } = new Dictionary<string, object>(StringComparer.Ordinal);

    // Don't serialize empty arrays
    public bool ShouldSerializeRequestUris() => RequestUris.Any();

    public bool ShouldSerializeDefaultAcrValues() => DefaultAcrValues.Any();

    public bool ShouldSerializeResponseTypes() => ResponseTypes.Any();

    public bool ShouldSerializeGrantTypes() => GrantTypes.Any();

    public bool ShouldSerializeContacts() => Contacts.Any();
}
