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
        NetworkHandler _successHandler;
        
        public DiscoveryPolicyTests()
        {
            var discoFileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "discovery.json");
            var document = File.ReadAllText(discoFileName);

            var jwksFileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "discovery_jwks.json");
            var jwks = File.ReadAllText(jwksFileName);

            _successHandler = new NetworkHandler(request =>
            {
                if (request.RequestUri.AbsoluteUri.EndsWith("jwks"))
                {
                    return jwks;
                }

                return document;
            }, HttpStatusCode.OK);
        }

        [Fact]
        public void authority_must_be_https()
        {
            var client = new DiscoveryClient("http://authority");
            client.Policy.RequireHttps = true;

            Func<Task> act = async () => { await client.GetAsync(); };

            act.ShouldThrow<InvalidOperationException>().Where(e => e.Message.Equals($"Policy demands the usage of HTTPS: http://authority/.well-known/openid-configuration"));
        }

        [Fact(Skip = "refine")]
        public async Task authority_can_be_http_if_allowed()
        {
            var client = new DiscoveryClient("http://authority");
            client.Policy.RequireHttps = false;
            client.Policy.ValidateIssuerName = false;
            client.Policy.ValidateEndpoints = false;

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task issuer_must_match_authority()
        {
            var client = new DiscoveryClient("https://differentissuer", _successHandler);
            client.Policy.ValidateIssuerName = true;

            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.PolicyViolation);
            disco.Error.Should().Be("Issuer name does not match authority");
        }

        [Fact]
        public async Task issuer_and_endpoint_can_be_unrelated_if_allowed()
        {
            var client = new DiscoveryClient("https://someotherhost", _successHandler);
            client.Policy.RequireHttps = true;
            client.Policy.ValidateIssuerName = false;
            client.Policy.ValidateEndpoints = false;

            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();
        }
    }
}