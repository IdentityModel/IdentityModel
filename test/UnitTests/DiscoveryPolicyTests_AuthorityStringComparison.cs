// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class DiscoveryPolicyTests_AuthorityStringComparison : DiscoveryPolicyTestsBase
    {
        public DiscoveryPolicyTests_AuthorityStringComparison() : base(new StringComparisonAuthorityValidationStrategy())
        {

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
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                AllowHttpOnLoopback = true
            });

            var client = new HttpClient(GetHandler(input));
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = input,
                Policy = policy
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Connecting_to_http_should_return_error()
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                AllowHttpOnLoopback = true
            });

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://authority",
                Policy = policy
            });

            disco.IsError.Should().BeTrue();
            disco.Json.ValueKind.Should().Be(JsonValueKind.Undefined);
            disco.ErrorType.Should().Be(ResponseErrorType.Exception);
            disco.Error.Should().StartWith("Error connecting to");
            disco.Error.Should().EndWith("HTTPS required.");
        }

        [Fact]
        public async Task If_policy_allows_http_non_http_must_not_return_error()
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = false
            });

            var client = new HttpClient(GetHandler("http://authority"));
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://authority",
                Policy = policy
            });

            disco.IsError.Should().BeFalse();
        }

        [Theory]
        [InlineData("http://localhost")]
        [InlineData("http://LocalHost")]
        [InlineData("http://127.0.0.1")]
        public async Task Http_on_loopback_must_not_return_error(string input)
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                AllowHttpOnLoopback = true
            });

            var client = new HttpClient(GetHandler(input));
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = input,
                Policy = policy
            });

            disco.IsError.Should().BeFalse();
        }


        [Fact]
        public async Task Invalid_issuer_name_must_return_policy_error()
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                ValidateIssuerName = true
            });

            var client = new HttpClient(GetHandler("https://differentissuer"));
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",
                Policy = policy
            });

            disco.IsError.Should().BeTrue();
            disco.Json.ValueKind.Should().Be(JsonValueKind.Undefined);
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Issuer name does not match authority");
        }

        [Fact]
        public async Task Excluded_endpoints_should_not_fail_validation()
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
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
                        "introspection_endpoint"
                    }
            });

            var handler = GetHandler("https://authority", "https://otherserver");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",
                Policy = policy
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Valid_issuer_name_must_return_no_error()
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                ValidateIssuerName = true
            });

            var handler = GetHandler("https://authority");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",
                Policy = policy
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Authority_comparison_may_be_case_insensitive()
        {
            var policy = new DiscoveryPolicy
            {
                ValidateIssuerName = true,
                AuthorityValidationStrategy = new StringComparisonAuthorityValidationStrategy(StringComparison.OrdinalIgnoreCase)
            };

            var handler = GetHandler("https://authority/tenantid");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority/TENANTID",
                Policy = policy
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Endpoints_not_using_https_should_return_policy_error()
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = true,
                ValidateEndpoints = true
            });

            var handler = GetHandler("https://authority", "http://authority");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",
                Policy = policy
            });

            disco.IsError.Should().BeTrue();
            disco.Json.ValueKind.Should().Be(JsonValueKind.Undefined);
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint does not use HTTPS");
        }

        [Theory]
        [InlineData("https://authority/sub", "https://authority")]
        [InlineData("https://authority/sub1", "https://authority/sub2")]
        public async Task Endpoints_not_beneath_authority_must_return_policy_error(string authority, string endpointBase)
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = true,
                ValidateEndpoints = true
            });

            var handler = GetHandler(authority, endpointBase);
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,
                Policy = policy
            });

            disco.IsError.Should().BeTrue();
            disco.Json.ValueKind.Should().Be(JsonValueKind.Undefined);
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint belongs to different authority");
        }

        [Theory]
        [InlineData("https://authority/sub", "https://authority")]
        [InlineData("https://authority/sub1", "https://authority/sub2")]
        public async Task Endpoints_not_beneath_authority_must_be_allowed_if_whitelisted(string authority, string endpointBase)
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = true,
                ValidateEndpoints = true,

                AdditionalEndpointBaseAddresses =
                    {
                        endpointBase
                    }
            });

            var handler = GetHandler(authority, endpointBase);
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,
                Policy = policy
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
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = true,
                ValidateEndpoints = true
            });

            var handler = GetHandler(authority, endpointBase);
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,
                Policy = policy
            });

            disco.IsError.Should().BeTrue();
            disco.Json.ValueKind.Should().Be(JsonValueKind.Undefined);
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
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = true,
                ValidateEndpoints = true,

                AdditionalEndpointBaseAddresses =
                    {
                        endpointBase
                    }
            });

            var handler = GetHandler(authority, endpointBase);
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = authority,
                Policy = policy
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Issuer_and_endpoint_can_be_unrelated_if_allowed()
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = true,
                ValidateEndpoints = false
            });

            var handler = GetHandler("https://authority", "https://differentauthority");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",
                Policy = policy
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Issuer_and_endpoint_can_be_unrelated_if_allowed_but_https_is_still_enforced()
        {
            var policy = ForceTestedAuthorityValidationStrategy(new DiscoveryPolicy
            {
                RequireHttps = true,
                ValidateIssuerName = true,
                ValidateEndpoints = false
            });

            var handler = GetHandler("https://authority", "http://differentauthority");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://authority",
                Policy = policy
            });

            disco.IsError.Should().BeTrue();
            disco.Json.ValueKind.Should().Be(JsonValueKind.Undefined);
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint does not use HTTPS");
        }
    }
}