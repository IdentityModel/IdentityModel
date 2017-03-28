// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Jwk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using IdentityModel.Internal;
using System.Linq;

#pragma warning disable 1591

namespace IdentityModel.Client
{
    /// <summary>
    /// Models the response from an OpenID Connect discovery endpoint
    /// </summary>
    public class DiscoveryResponse
    {
        /// <summary>
        /// Gets the raw response.
        /// </summary>
        /// <value>
        /// The raw.
        /// </value>
        public string Raw { get; }

        /// <summary>
        /// Gets the response as a JObject.
        /// </summary>
        /// <value>
        /// The json.
        /// </value>
        public JObject Json { get; }

        /// <summary>
        /// Gets a value indicating whether an error occurred.
        /// </summary>
        /// <value>
        ///   <c>true</c> if an error occurred; otherwise, <c>false</c>.
        /// </value>
        public bool IsError { get; } = false;

        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; }

        /// <summary>
        /// Gets or sets the type of the error.
        /// </summary>
        /// <value>
        /// The type of the error.
        /// </value>
        public ResponseErrorType ErrorType { get; set; } = ResponseErrorType.None;

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; }

        /// <summary>
        /// Gets or sets the JSON web key set.
        /// </summary>
        /// <value>
        /// The key set.
        /// </value>
        public JsonWebKeySet KeySet { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryResponse"/> class.
        /// </summary>
        /// <param name="raw">The raw response.</param>
        /// <param name="policy">The security policy.</param>
        public DiscoveryResponse(string raw, DiscoveryPolicy policy = null)
        {
            if (policy == null) policy = new DiscoveryPolicy();

            IsError = false;
            StatusCode = HttpStatusCode.OK;
            Raw = raw;

            try
            {
                Json = JObject.Parse(raw);
                var validationError = Validate(policy);

                if (!string.IsNullOrEmpty(validationError))
                {
                    IsError = true;
                    Json = null;

                    ErrorType = ResponseErrorType.PolicyViolation;
                    Error = validationError;
                }
            }
            catch (Exception ex)
            {
                IsError = true;

                ErrorType = ResponseErrorType.Exception;
                Error = ex.Message;
                Exception = ex;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reason">The reason.</param>
        public DiscoveryResponse(HttpStatusCode statusCode, string reason)
        {
            IsError = true;

            ErrorType = ResponseErrorType.Http;
            StatusCode = statusCode;
            Error = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryResponse"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="errorMessage">The error message.</param>
        public DiscoveryResponse(Exception exception, string errorMessage)
        {
            IsError = true;

            ErrorType = ResponseErrorType.Exception;
            Exception = exception;
            Error = $"{errorMessage}: {exception.Message}";
        }

        // strongly typed
        public string Issuer => TryGetString(OidcConstants.Discovery.Issuer);
        public string AuthorizeEndpoint => TryGetString(OidcConstants.Discovery.AuthorizationEndpoint);
        public string TokenEndpoint => TryGetString(OidcConstants.Discovery.TokenEndpoint);
        public string UserInfoEndpoint => TryGetString(OidcConstants.Discovery.UserInfoEndpoint);
        public string IntrospectionEndpoint => TryGetString(OidcConstants.Discovery.IntrospectionEndpoint);
        public string RevocationEndpoint => TryGetString(OidcConstants.Discovery.RevocationEndpoint);
        public string JwksUri => TryGetString(OidcConstants.Discovery.JwksUri);
        public string EndSessionEndpoint => TryGetString(OidcConstants.Discovery.EndSessionEndpoint);
        public string CheckSessionIframe => TryGetString(OidcConstants.Discovery.CheckSessionIframe);
        public string RegistrationEndpoint => TryGetString(OidcConstants.Discovery.RegistrationEndpoint);
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

        // generic
        public JToken TryGetValue(string name) => Json.TryGetValue(name);
        public string TryGetString(string name) => Json.TryGetString(name);
        public bool? TryGetBoolean(string name) => Json.TryGetBoolean(name);
        public IEnumerable<string> TryGetStringArray(string name) => Json.TryGetStringArray(name);

        private string Validate(DiscoveryPolicy policy)
        {
            if (policy.ValidateIssuerName)
            {
                if (string.IsNullOrWhiteSpace(Issuer)) return "Issuer name is missing";

                var isValid = ValidateIssuerName(Issuer.RemoveTrailingSlash(), policy.Authority.RemoveTrailingSlash());
                if (!isValid) return "Issuer name does not match authority: " + Issuer;
            }

            var error = ValidateEndoints(Json, policy);
            if (!string.IsNullOrEmpty(error)) return error;

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
            return string.Equals(issuer, authority, StringComparison.Ordinal);
        }

        /// <summary>
        /// Validates the endoints and jwks_uri according to the security policy.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        public string ValidateEndoints(JObject json, DiscoveryPolicy policy)
        {
            // allowed hosts
            var allowedHosts = new HashSet<string>(policy.AdditionalEndpointBaseAddresses.Select(e => new Uri(e).Authority));
            allowedHosts.Add(new Uri(policy.Authority).Authority);

            // allowed authorities (hosts + base address)
            var allowedAuthorities = new HashSet<string>(policy.AdditionalEndpointBaseAddresses);
            allowedAuthorities.Add(policy.Authority);

            foreach (var element in json)
            {
                if (element.Key.EndsWith("endpoint", StringComparison.OrdinalIgnoreCase) ||
                    element.Key.Equals(OidcConstants.Discovery.JwksUri, StringComparison.OrdinalIgnoreCase) ||
                    element.Key.Equals(OidcConstants.Discovery.CheckSessionIframe, StringComparison.OrdinalIgnoreCase))
                {
                    var endpoint = element.Value.ToString();

                    var isValidUri = Uri.TryCreate(endpoint, UriKind.Absolute, out Uri uri);
                    if (!isValidUri)
                    {
                        return $"Malformed endpoint: {endpoint}";
                    }

                    if (!DiscoveryUrlHelper.IsValidScheme(uri))
                    {
                        return $"Malformed endpoint: {endpoint}";
                    }

                    if (!DiscoveryUrlHelper.IsSecureScheme(uri, policy))
                    {
                        return $"Endpoint does not use HTTPS: {endpoint}";
                    }

                    if (policy.ValidateEndpoints)
                    {
                        // if endpoint is on exclude list, don't validate
                        if (policy.EndpointValidationExcludeList.Contains(element.Key)) continue;

                        bool isAllowed = false;
                        foreach (var host in allowedHosts)
                        {
                            if (string.Equals(host, uri.Authority))
                            {
                                isAllowed = true;
                            }
                        }

                        if (!isAllowed)
                        {
                            return $"Endpoint is on a different host than authority: {endpoint}";
                        }


                        isAllowed = false;
                        foreach (var authority in allowedAuthorities)
                        {
                            if (endpoint.StartsWith(authority, StringComparison.Ordinal))
                            {
                                isAllowed = true;
                            }
                        }

                        if (!isAllowed)
                        {
                            return $"Endpoint belongs to different authority: {endpoint}";
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
}