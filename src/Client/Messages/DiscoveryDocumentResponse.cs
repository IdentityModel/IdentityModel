// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using IdentityModel.Jwk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace IdentityModel.Client;

/// <summary>
/// Models the response from an OpenID Connect discovery endpoint
/// </summary>
public class DiscoveryDocumentResponse : ProtocolResponse
{
    public DiscoveryPolicy Policy { get; set; } = default!;

    protected override Task InitializeAsync(object? initializationData = null)
    {
        if (!HttpResponse.IsSuccessStatusCode)
        {
            ErrorMessage = initializationData as string;
            return Task.CompletedTask;
        }

        Policy = initializationData as DiscoveryPolicy ?? new DiscoveryPolicy();

        var validationError = Validate(Policy);

        if (validationError.IsPresent())
        {
            Json = default;

            ErrorType = ResponseErrorType.PolicyViolation;
            ErrorMessage = validationError;
        }

        MtlsEndpointAliases =
            new MtlsEndpointAliases(Json.TryGetValue(OidcConstants.Discovery.MtlsEndpointAliases));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets or sets the JSON web key set.
    /// </summary>
    /// <value>
    /// The key set.
    /// </value>
    public JsonWebKeySet? KeySet { get; set; }
        
    /// <summary>
    /// Gets the MTLS endpoint aliases
    /// </summary>
    /// <value>
    /// The key set.
    /// </value>
    public MtlsEndpointAliases? MtlsEndpointAliases { get; internal set; }
        
    // strongly typed
    public string? Issuer => TryGetString(OidcConstants.Discovery.Issuer);
    public string? AuthorizeEndpoint => TryGetString(OidcConstants.Discovery.AuthorizationEndpoint);
    public string? TokenEndpoint => TryGetString(OidcConstants.Discovery.TokenEndpoint);
    public string? UserInfoEndpoint => TryGetString(OidcConstants.Discovery.UserInfoEndpoint);
    public string? IntrospectionEndpoint => TryGetString(OidcConstants.Discovery.IntrospectionEndpoint);
    public string? RevocationEndpoint => TryGetString(OidcConstants.Discovery.RevocationEndpoint);
    public string? DeviceAuthorizationEndpoint => TryGetString(OidcConstants.Discovery.DeviceAuthorizationEndpoint);
    public string? BackchannelAuthenticationEndpoint => TryGetString(OidcConstants.Discovery.BackchannelAuthenticationEndpoint);
    
    public string? JwksUri => TryGetString(OidcConstants.Discovery.JwksUri);
    public string? EndSessionEndpoint => TryGetString(OidcConstants.Discovery.EndSessionEndpoint);
    public string? CheckSessionIframe => TryGetString(OidcConstants.Discovery.CheckSessionIframe);
    public string? RegistrationEndpoint => TryGetString(OidcConstants.Discovery.RegistrationEndpoint);
    public bool? FrontChannelLogoutSupported => TryGetBoolean(OidcConstants.Discovery.FrontChannelLogoutSupported);
    public bool? FrontChannelLogoutSessionSupported => TryGetBoolean(OidcConstants.Discovery.FrontChannelLogoutSessionSupported);
    public IEnumerable<string> GrantTypesSupported => TryGetStringArray(OidcConstants.Discovery.GrantTypesSupported);
    public IEnumerable<string> CodeChallengeMethodsSupported => TryGetStringArray(OidcConstants.Discovery.CodeChallengeMethodsSupported);
    public IEnumerable<string> ScopesSupported => TryGetStringArray(OidcConstants.Discovery.ScopesSupported);
    public IEnumerable<string> SubjectTypesSupported => TryGetStringArray(OidcConstants.Discovery.SubjectTypesSupported);
    public IEnumerable<string> ResponseModesSupported => TryGetStringArray(OidcConstants.Discovery.ResponseModesSupported);
    public IEnumerable<string> ResponseTypesSupported => TryGetStringArray(OidcConstants.Discovery.ResponseTypesSupported);
    public IEnumerable<string> ClaimsSupported => TryGetStringArray(OidcConstants.Discovery.ClaimsSupported);
    public IEnumerable<string> TokenEndpointAuthenticationMethodsSupported => TryGetStringArray(OidcConstants.Discovery.TokenEndpointAuthenticationMethodsSupported);
    public IEnumerable<string> BackchannelTokenDeliveryModesSupported => TryGetStringArray(OidcConstants.Discovery.BackchannelTokenDeliveryModesSupported);
    public bool? BackchannelUserCodeParameterSupported => TryGetBoolean(OidcConstants.Discovery.BackchannelUserCodeParameterSupported);
    
    // generic
    public JsonElement TryGetValue(string name) => Json.TryGetValue(name);
    public string? TryGetString(string name) => Json.TryGetString(name);
    public bool? TryGetBoolean(string name) => Json.TryGetBoolean(name);
    public IEnumerable<string> TryGetStringArray(string name) => Json.TryGetStringArray(name);

    private string Validate(DiscoveryPolicy policy)
    {
        if (policy.ValidateIssuerName)
        {
            IAuthorityValidationStrategy strategy = policy.AuthorityValidationStrategy ?? DiscoveryPolicy.DefaultAuthorityValidationStrategy;

            AuthorityValidationResult issuerValidationResult = strategy.IsIssuerNameValid(Issuer!, policy.Authority);

            if (!issuerValidationResult.Success)
            {
                return issuerValidationResult.ErrorMessage;
            }
        }

        var error = ValidateEndpoints(Json, policy);
        if (error.IsPresent())
        {
            return error;
        }

        return string.Empty;
    }

    /// <summary>
    /// Checks if the issuer matches the authority.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="authority">The authority.</param>
    /// <returns></returns>
    public bool ValidateIssuerName(string issuer, string authority)
    {
        return DiscoveryPolicy.DefaultAuthorityValidationStrategy.IsIssuerNameValid(issuer, authority).Success;
    }

    /// <summary>
    /// Checks if the issuer matches the authority.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="authority">The authority.</param>
    /// <param name="nameComparison">The comparison mechanism that should be used when performing the match.</param>
    /// <returns></returns>
    public bool ValidateIssuerName(string issuer, string authority, StringComparison nameComparison)
    {
        return new StringComparisonAuthorityValidationStrategy(nameComparison).IsIssuerNameValid(issuer, authority).Success;
    }

    /// <summary>
    /// Checks if the issuer matches the authority.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="authority">The authority.</param>
    /// <param name="validationStrategy">The strategy to use.</param>
    /// <returns></returns>
    private bool ValidateIssuerName(string issuer, string authority, IAuthorityValidationStrategy validationStrategy)
    {
        return validationStrategy.IsIssuerNameValid(issuer, authority).Success;
    }
    
    /// <summary>
    /// Validates the endoints and jwks_uri according to the security policy.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="policy">The policy.</param>
    /// <returns></returns>
    public string ValidateEndpoints(JsonElement json, DiscoveryPolicy policy)
    {
        // allowed hosts
        var allowedHosts = new HashSet<string>(policy.AdditionalEndpointBaseAddresses.Select(e => new Uri(e).Authority))
        {
            new Uri(policy.Authority).Authority
        };

        // allowed authorities (hosts + base address)
        var allowedAuthorities = new HashSet<string>(policy.AdditionalEndpointBaseAddresses)
        {
            policy.Authority
        };
            
        foreach (var element in json.EnumerateObject())
        {
            if (element.Name.EndsWith("endpoint", StringComparison.OrdinalIgnoreCase) ||
                element.Name.Equals(OidcConstants.Discovery.JwksUri, StringComparison.OrdinalIgnoreCase) ||
                element.Name.Equals(OidcConstants.Discovery.CheckSessionIframe, StringComparison.OrdinalIgnoreCase))
            {
                var endpoint = element.Value.ToString();
            
                var isValidUri = Uri.TryCreate(endpoint, UriKind.Absolute, out var uri);
                if (!isValidUri)
                {
                    return $"Malformed endpoint: {endpoint}";
                }
            
                if (!DiscoveryEndpoint.IsValidScheme(uri!))
                {
                    return $"Malformed endpoint: {endpoint}";
                }
            
                if (!DiscoveryEndpoint.IsSecureScheme(uri!, policy))
                {
                    return $"Endpoint does not use HTTPS: {endpoint}";
                }
            
                if (policy.ValidateEndpoints)
                {
                    // if endpoint is on exclude list, don't validate
                    if (policy.EndpointValidationExcludeList.Contains(element.Name))
                    {
                        continue;
                    }
            
                    bool isAllowed = false;
                    foreach (var host in allowedHosts)
                    {
                        if (string.Equals(host, uri!.Authority))
                        {
                            isAllowed = true;
                        }
                    }
            
                    if (!isAllowed)
                    {
                        return $"Endpoint is on a different host than authority: {endpoint}";
                    }
            
                    IAuthorityValidationStrategy strategy = policy.AuthorityValidationStrategy ?? DiscoveryPolicy.DefaultAuthorityValidationStrategy;
                    AuthorityValidationResult endpointValidationResult = strategy.IsEndpointValid(endpoint, allowedAuthorities);
                    if (!endpointValidationResult.Success)
                    {
                        return endpointValidationResult.ErrorMessage;
                    }
                }
            }
        }

        if (policy.RequireKeySet)
        {
            if (string.IsNullOrWhiteSpace(JwksUri))
            {
                return "Keyset is missing";
            }
        }

        return string.Empty;
    }
}