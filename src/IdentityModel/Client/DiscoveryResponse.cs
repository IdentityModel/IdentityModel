// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Jwk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Schema;
using IdentityModel.Internal;

namespace IdentityModel.Client
{
    public class DiscoveryResponse
    {
        public string Raw { get; }
        public JObject Json { get; }

        public bool IsError { get; } = false;
        public HttpStatusCode StatusCode { get; }
        public string Error { get; }
        public ResponseErrorType ErrorType { get; set; } = ResponseErrorType.None;
        public Exception Exception { get; }

        public JsonWebKeySet KeySet { get; set; }

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

        public DiscoveryResponse(HttpStatusCode statusCode, string reason)
        {
            IsError = true;

            ErrorType = ResponseErrorType.Http;
            StatusCode = statusCode;
            Error = reason;
        }

        public DiscoveryResponse(Exception exception)
        {
            IsError = true;

            ErrorType = ResponseErrorType.Exception;
            Exception = exception;
            Error = exception.Message;
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
                var isValid = ValidateIssuerName(Issuer.RemoveTrailingSlash(), policy.Authority.RemoveTrailingSlash());
                if (!isValid) return "Issuer name does not match authority";
            }

            var error = ValidateEndoints(Json, policy);
            if (!string.IsNullOrEmpty(error)) return error;

            return string.Empty;
        }

        public bool ValidateIssuerName(string issuer, string authority)
        {
            return string.Equals(issuer, authority, StringComparison.Ordinal);
        }

        public string ValidateEndoints(JObject json, DiscoveryPolicy policy)
        {
            var authorityHost = new Uri(policy.Authority).Authority;

            foreach (var element in json)
            {
                if (element.Key.EndsWith("Endpoint", StringComparison.OrdinalIgnoreCase) ||
                    element.Key.Equals(OidcConstants.Discovery.JwksUri, StringComparison.OrdinalIgnoreCase))
                {
                    var endpoint = element.Value.ToString();
                    Uri uri;

                    var isValidUri = Uri.TryCreate(endpoint, UriKind.Absolute, out uri);
                    if (!isValidUri)
                    {
                        return $"Malformed endpoint: {endpoint}";
                    }

                    if (!DiscoveryUrlHelper.IsSecureScheme(uri, policy))
                    {
                        return $"Endpoint does not use HTTPS: {endpoint}";
                    }

                    if (policy.ValidateEndpoints)
                    {
                        if (!string.Equals(authorityHost, uri.Host))
                        {
                            return $"Endpoint is on a different host than authority: {endpoint}";
                        }

                        //if (!endpoint.StartsWith(policy.Authority, StringComparison.Ordinal))
                        //{
                        //    return $"Endpoint not beneath authority: {endpoint}";
                        //}
                    }
                }
            }

            return string.Empty;
        }
    }
}