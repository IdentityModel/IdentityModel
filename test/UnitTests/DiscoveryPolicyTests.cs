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
        private NetworkHandler GetHandler(string issuer, string endpointBase = null)
        {
            if (endpointBase == null) endpointBase = issuer;

            var discoFileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "discovery_variable.json");
            var raw = File.ReadAllText(discoFileName);

            var document = raw.Replace("{issuer}", issuer).Replace("{endpointBase}", endpointBase);

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

        [Fact]
        public void if_policy_requires_https_non_https_must_throw()
        {
            var client = new DiscoveryClient("http://authority");
            client.Policy.RequireHttps = true;

            Func<Task> act = async () => { await client.GetAsync(); };

            act.ShouldThrow<InvalidOperationException>().Where(e => e.Message.Equals($"Policy demands the usage of HTTPS: http://authority/.well-known/openid-configuration"));
        }

        [Fact]
        public void if_policy_allows_http_non_http_must_not_throw()
        {
            var client = new DiscoveryClient("http://authority");
            client.Policy.RequireHttps = false;

            Func<Task> act = async () => { await client.GetAsync(); };

            act.ShouldNotThrow();
        }

        [Fact]
        public void if_policy_requires_https_localhost_http_must_not_throw()
        {
            var client = new DiscoveryClient("http://localhost", GetHandler("http://localhost"));
            client.Policy.RequireHttps = true;
            client.Policy.AllowHttpOnLoopback = true;

            Func<Task> act = async () => { await client.GetAsync(); };

            act.ShouldNotThrow();
        }

        [Fact]
        public void if_policy_requires_https_127_0_0_1_http_must_not_throw()
        {
            var client = new DiscoveryClient("http://127.0.0.1", GetHandler("http://127.0.0.1"));
            client.Policy.RequireHttps = true;
            client.Policy.AllowHttpOnLoopback = true;

            Func<Task> act = async () => { await client.GetAsync(); };

            act.ShouldNotThrow();
        }

        [Fact]
        public async Task issuer_must_match_authority()
        {
            var handler = GetHandler("https://differentissuer");
            var client = new DiscoveryClient("https://authority", handler);
            client.Policy.ValidateIssuerName = true;

            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().Be("Issuer name does not match authority");
        }

        [Fact]
        public async Task issuer_and_endpoint_can_be_unrelated_if_allowed()
        {
            var handler = GetHandler("https://differentissuer");
            var client = new DiscoveryClient("https://authority", handler);

            client.Policy.RequireHttps = true;
            client.Policy.ValidateIssuerName = false;
            client.Policy.ValidateEndpoints = false;

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }
    }
}