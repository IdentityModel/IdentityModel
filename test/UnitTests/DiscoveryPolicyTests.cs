using FluentAssertions;
using IdentityModel.Client;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Net;
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

            var discoFileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "discovery_variable.json");
            var raw = File.ReadAllText(discoFileName);

            var document = raw.Replace("{issuer}", issuer)
                              .Replace("{endpointBase}", endpointBase)
                              .Replace("{alternateEndpointBase}", alternateEndpointBase);

            var jwksFileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "discovery_jwks.json");
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
        [InlineData("http://localhost")]
        [InlineData("http://LocalHost")]
        [InlineData("http://127.0.0.1")]
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
        public async Task success_with_default_policy(string input)
        {
            var client = new DiscoveryClient(input, GetHandler(input))
            {
                Policy =
                {
                    RequireHttps = true,
                    AllowHttpOnLoopback = true
                }
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task connecting_to_http_should_return_error()
        {
            var client = new DiscoveryClient("http://authority")
            {
                Policy =
                {
                    RequireHttps = true,
                    AllowHttpOnLoopback = true
                }
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.Exception);
            disco.Error.Should().StartWith("Error connecting to");
            disco.Error.Should().EndWith("HTTPS required");
        }

        [Fact]
        public async Task if_policy_allows_http_non_http_must_not_return_error()
        {
            var client = new DiscoveryClient("http://authority", GetHandler("http://authority"))
            {
                Policy = {RequireHttps = false}
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Theory]
        [InlineData("http://localhost")]
        [InlineData("http://LocalHost")]
        [InlineData("http://127.0.0.1")]
        public async Task http_on_loopback_must_not_return_error(string input)
        {
            var client = new DiscoveryClient(input, GetHandler(input))
            {
                Policy =
                {
                    RequireHttps = true,
                    AllowHttpOnLoopback = true
                }
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        
        [Fact]
        public async Task invalid_issuer_name_must_return_policy_error()
        {
            var handler = GetHandler("https://differentissuer");
            var client = new DiscoveryClient("https://authority", handler)
            {
                Policy = {ValidateIssuerName = true}
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Issuer name does not match authority");
        }

        [Fact]
        public async Task excluded_endpoints_should_not_fail_validation()
        {
            var handler = GetHandler("https://authority", "https://otherserver");
            var client = new DiscoveryClient("https://authority", handler)
            {
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
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task valid_issuer_name_must_return_no_error()
        {
            var handler = GetHandler("https://authority");
            var client = new DiscoveryClient("https://authority", handler)
            {
                Policy = {ValidateIssuerName = true}
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task endpoints_not_using_https_should_return_policy_error()
        {
            var handler = GetHandler("https://authority", "http://authority");
            var client = new DiscoveryClient("https://authority", handler)
            {
                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = true
                }
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint does not use HTTPS");
        }

        [Theory]
        [InlineData("https://authority/sub", "https://authority")]
        [InlineData("https://authority/sub1", "https://authority/sub2")]
        public async Task endpoints_not_beneath_authority_must_return_policy_error(string authority, string endpointBase)
        {
            var handler = GetHandler(authority, endpointBase);
            var client = new DiscoveryClient(authority, handler)
            {
                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = true
                }
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint belongs to different authority");
        }

        [Theory]
        [InlineData("https://authority/sub", "https://authority")]
        [InlineData("https://authority/sub1", "https://authority/sub2")]
        public async Task endpoints_not_beneath_authority_must_be_allowed_if_whitelisted(string authority, string endpointBase)
        {
            var handler = GetHandler(authority, endpointBase);
            var client = new DiscoveryClient(authority, handler)
            {
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
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Theory]
        [InlineData("https://authority", "https://differentauthority")]
        [InlineData("https://authority/sub", "https://differentauthority")]
        [InlineData("https://127.0.0.1", "https://differentauthority")]
        [InlineData("https://127.0.0.1", "https://127.0.0.2")]
        [InlineData("https://127.0.0.1", "https://localhost")]
        public async Task endpoints_not_belonging_to_authority_host_must_return_policy_error(string authority, string endpointBase)
        {
            var handler = GetHandler(authority, endpointBase);
            var client = new DiscoveryClient(authority, handler)
            {
                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = true
                }
            };

            var disco = await client.GetAsync();

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
        public async Task endpoints_not_belonging_to_authority_host_must_be_allowed_if_whitelisted(string authority, string endpointBase)
        {
            var handler = GetHandler(authority, endpointBase);
            var client = new DiscoveryClient(authority, handler)
            {
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
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task issuer_and_endpoint_can_be_unrelated_if_allowed()
        {
            var handler = GetHandler("https://authority", "https://differentauthority");
            var client = new DiscoveryClient("https://authority", handler)
            {
                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = false
                }
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task issuer_and_endpoint_can_be_unrelated_if_allowed_but_https_is_still_enforced()
        {
            var handler = GetHandler("https://authority", "http://differentauthority");
            var client = new DiscoveryClient("https://authority", handler)
            {
                Policy =
                {
                    RequireHttps = true,
                    ValidateIssuerName = true,
                    ValidateEndpoints = false
                }
            };

            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.Json.Should().BeNull();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().StartWith("Endpoint does not use HTTPS");
        }
    }
}