// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Jwk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace IdentityModel.Client
{
    public class DiscoveryResponse
    {
        public string Raw { get; }
        public JObject Json { get; }

        public bool IsError { get; } = false;
        public HttpStatusCode StatusCode { get; }
        public string Error { get; set; }

        public JsonWebKeySet KeySet { get; set; }

        public DiscoveryResponse(string raw)
        {
            IsError = false;
            StatusCode = HttpStatusCode.OK;

            Raw = raw;
            Json = JObject.Parse(raw);
        }

        public DiscoveryResponse(HttpStatusCode statusCode, string error)
        {
            IsError = true;
            StatusCode = statusCode;
            Error = error;
        }

        public string Issuer                                                   => TryGetString(OidcConstants.Discovery.Issuer);
        public string AuthorizationEndpoint                                    => TryGetString(OidcConstants.Discovery.AuthorizationEndpoint);
        public string TokenEndpoint                                            => TryGetString(OidcConstants.Discovery.TokenEndpoint);
        public string UserInfoEndpoint                                         => TryGetString(OidcConstants.Discovery.UserInfoEndpoint);
        public string IntrospectionEndpoint                                    => TryGetString(OidcConstants.Discovery.IntrospectionEndpoint);
        public string RevocationEndpoint                                       => TryGetString(OidcConstants.Discovery.RevocationEndpoint);
        public string JwksUri                                                  => TryGetString(OidcConstants.Discovery.JwksUri);
        public string EndSessionEndpoint                                       => TryGetString(OidcConstants.Discovery.EndSessionEndpoint);
        public string CheckSessionIframe                                       => TryGetString(OidcConstants.Discovery.CheckSessionIframe);
        public string RegistrationEndpoint                                     => TryGetString(OidcConstants.Discovery.RegistrationEndpoint);
        public bool? FrontChannelLogoutSupported                               => TryGetBoolean(OidcConstants.Discovery.FrontChannelLogoutSupported);
        public bool? FrontChannelLogoutSessionSupported                        => TryGetBoolean(OidcConstants.Discovery.FrontChannelLogoutSessionSupported);
        public IEnumerable<string> GrantTypesSupported                         => TryGetStringArray(OidcConstants.Discovery.GrantTypesSupported);
        public IEnumerable<string> CodeChallengeMethodsSupported               => TryGetStringArray(OidcConstants.Discovery.CodeChallengeMethodsSupported);
        public IEnumerable<string> ScopesSupported                             => TryGetStringArray(OidcConstants.Discovery.ScopesSupported);
        public IEnumerable<string> SubjectTypesSupported                       => TryGetStringArray(OidcConstants.Discovery.SubjectTypesSupported);
        public IEnumerable<string> ResponseModesSupported                      => TryGetStringArray(OidcConstants.Discovery.ResponseModesSupported);
        public IEnumerable<string> ResponseTypesSupported                      => TryGetStringArray(OidcConstants.Discovery.ResponseTypesSupported);
        public IEnumerable<string> ClaimsSupported                             => TryGetStringArray(OidcConstants.Discovery.ClaimsSupported);
        public IEnumerable<string> TokenEndpointAuthenticationMethodsSupported => TryGetStringArray(OidcConstants.Discovery.TokenEndpointAuthenticationMethodsSupported);

        public JToken TryGetValue(string name)
        {
            JToken value;
            if (Json != null && Json.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out value))
            {
                return value;
            }

            return null;
        }

        public string TryGetString(string name)
        {
            JToken value = TryGetValue(name);
            return value?.ToString() ?? null;
        }

        public bool? TryGetBoolean(string name)
        {
            var value = TryGetString(name);

            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public IEnumerable<string> TryGetStringArray(string name)
        {
            var values = new List<string>();

            var array = TryGetValue(name) as JArray;
            if (array != null)
            {
                foreach (var item in array)
                {
                    values.Add(item.ToString());
                }
            }

            return values;
        }
    }
}