// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class DiscoveryPolicyTests
    {
        private NetworkHandler GetHandler(string issuer, string endpointBase = null, string alternateEndpointBase = null)
        {
            if (endpointBase == null) endpointBase = issuer;
            if (alternateEndpointBase == null) alternateEndpointBase = issuer;

            var discoFileName = FileName.Create("discovery_variable.json");
            var raw = File.ReadAllText(discoFileName);

            var document = raw.Replace("{issuer}", issuer)
                              .Replace("{endpointBase}", endpointBase)
                              .Replace("{alternateEndpointBase}", alternateEndpointBase);

            var jwksFileName = FileName.Create("discovery_jwks.json");
            var jwks = File.ReadAllText(jwksFileName);

            var handler = new NetworkHandler(request =>
            {
                if (request.RequestUri.AbsoluteUri.EndsWith("jwks"))
                {
                    return jwks;
                }

                return document;
            }, HttpStatusCode.OK);

            return handler;
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("file://some_file")]
        [InlineData("https:something_weird_https://something_other")]
        public void Malformed_authority_url_should_throw(string input)
        {
            Action act = () => DiscoveryEndpoint.ParseUrl(input);

            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.Equals("Malformed URL"));
        }

        [Theory]
        [InlineData("https://server:123/.well-known/openid-configuration")]
        [InlineData("https://server:123/.well-known/openid-configuration/")]
        [InlineData("https://server:123/")]
        [InlineData("https://server:123")]
        public void Various_urls_should_normalize(string input)
        {
            var result = DiscoveryEndpoint.ParseUrl(input);

            // test parse URL logic
            result.Url.Should().Be("https://server:123/.well-known/openid-configuration");
            result.Authority.Should().Be("https://server:123");
        }

        [Theory]
        [InlineData("http://localhost")]
        [InlineData("http://LocalHost")]
        [InlineData("http://127.0.0.1")]
        [InlineData("http://localhost:5000")]
        [InlineData("http://LocalHost:5000")]
        [InlineData("http://127.0.0.1:5000")]
        [InlineData("https://authority")]
        [InlineData("https://authority:5000")]
        [InlineData("https://authority/sub")]
        [InlineData("https://authority:5000/sub")]
        [InlineData("https://demo.identityserver.io")]
        [InlineData("https://sub.demo.identityserver.io")]
        [InlineData("https://demo.identityserver.io/sub")]
        [InlineData("https://demo.identityserver.io:5000/sub")]
        [InlineData("https://sub.demo.identityserver.io:5000/sub")]
        public async Task Valid_Urls_with_default_policy_should_succeed(string input)
        {
            var client = new HttpClient(GetHandler(input));
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = input,

                Policy =
                {
                    RequireHttps = true,
                    AllowHttpOnLoopback = true
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Connecting_to_http_should_return_error()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://authority",

                Policy =
                {
                    RequireHttps = true,
                    AllowHttpOnLoopback = true
                }
            });

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.Exception);
            disco.Error.Should().StartWith("Error connecting to");
            disco.Error.Should().EndWith("HTTPS required.");
        }

        [Fact]
        public async Task If_policy_allows_http_non_http_must_not_return_error()
        {
            var client = new HttpClient(GetHandler("http://authority"));
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://authority",

                Policy =
                {
                    RequireHttps = false
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Theory]
        [InlineData("http://localhost")]
        [InlineData("http://LocalHost")]
        [InlineData("http://127.0.0.1")]
        public async Task Http_on_loopback_must_not_return_error(string input)
        {
            var client = new HttpClient(GetHandler(input));
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = input,

                Policy =
                {
                    RequireHttps = true,
                    AllowHttpOnLoopback = true
                }
            });

            disco.IsError.Should().BeFalse();
        }


        [Fact]
        public async Task Invalid_issuer_name_must_return_policy_error()
        {
            var client = new HttpClient(GetHandler("https://differentissuer"));
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",

                Policy =
                {
                    ValidateIssuerName = true
                }
            });

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Issuer name does not match authority");
        }

        [Fact]
        public async Task Excluded_endpoints_should_not_fail_validation()
        {
            var handler = GetHandler("https://authority", "https://otherserver");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",

                Policy =
                {
                    ValidateEndpoints = true,
                    EndpointValidationExcludeList =
                    {
                        "jwks_uri",
                        "authorization_endpoint",
                        "token_endpoint",
                        "userinfo_endpoint",
                        "end_session_endpoint",
                        "check_session_iframe",
                        "revocation_endpoint",
                        "introspection_endpoint",
                    }
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Valid_issuer_name_must_return_no_error()
        {
            var handler = GetHandler("https://authority");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",

                Policy =
                {
                    ValidateIssuerName = true
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Authority_comparison_may_be_case_insensitive()
        {
            var handler = GetHandler("https://authority/tenantid");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority/TENANTID",

                Policy =
                {
                    ValidateIssuerName = true,
                    AuthorityValidationStrategy = new StringComparisonAuthorityValidationStrategy(StringComparison.OrdinalIgnoreCase)
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Authority_comparison_with_uri_equivalence()
        {
            var handler = GetHandler(issuer: "https://authority:443/tenantid/");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority/tenantid",

                Policy =
                {
                    ValidateIssuerName = true,
                    AuthorityValidationStrategy = AuthorityUrlValidationStrategy.Instance
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Endpoints_not_using_https_should_return_policy_error()
        {
            var handler = GetHandler("https://authority", "http://authority");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",

                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = true
                }
            });

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint does not use HTTPS");
        }

        [Theory]
        [InlineData("https://authority/sub", "https://authority")]
        [InlineData("https://authority/sub1", "https://authority/sub2")]
        public async Task Endpoints_not_beneath_authority_must_return_policy_error(string authority, string endpointBase)
        {
            var handler = GetHandler(authority, endpointBase);
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,

                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = true
                }
            });

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint belongs to different authority");
        }

        [Theory]
        [InlineData("https://authority/sub", "https://authority")]
        [InlineData("https://authority/sub1", "https://authority/sub2")]
        public async Task Endpoints_not_beneath_authority_must_be_allowed_if_whitelisted(string authority, string endpointBase)
        {
            var handler = GetHandler(authority, endpointBase);
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,

                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = true,

                    AdditionalEndpointBaseAddresses =
                    {
                        endpointBase
                    }
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Theory]
        [InlineData("https://authority", "https://differentauthority")]
        [InlineData("https://authority/sub", "https://differentauthority")]
        [InlineData("https://127.0.0.1", "https://differentauthority")]
        [InlineData("https://127.0.0.1", "https://127.0.0.2")]
        [InlineData("https://127.0.0.1", "https://localhost")]
        public async Task Endpoints_not_belonging_to_authority_host_must_return_policy_error(string authority, string endpointBase)
        {
            var handler = GetHandler(authority, endpointBase);
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,

                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = true
                }
            });

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint is on a different host than authority");
        }

        [Theory]
        [InlineData("https://authority", "https://differentauthority")]
        [InlineData("https://authority/sub", "https://differentauthority")]
        [InlineData("https://127.0.0.1", "https://differentauthority")]
        [InlineData("https://127.0.0.1", "https://127.0.0.2")]
        [InlineData("https://127.0.0.1", "https://localhost")]
        public async Task Endpoints_not_belonging_to_authority_host_must_be_allowed_if_whitelisted(string authority, string endpointBase)
        {
            var handler = GetHandler(authority, endpointBase);
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,

                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = true,

                    AdditionalEndpointBaseAddresses =
                    {
                        endpointBase
                    }
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Issuer_and_endpoint_can_be_unrelated_if_allowed()
        {
            var handler = GetHandler("https://authority", "https://differentauthority");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",

                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = false
                }
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Issuer_and_endpoint_can_be_unrelated_if_allowed_but_https_is_still_enforced()
        {
            var handler = GetHandler("https://authority", "http://differentauthority");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",

                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = false
                }
            });

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint does not use HTTPS");
        }
    }
}