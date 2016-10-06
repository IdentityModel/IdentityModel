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
    public class DiscoveryClientTests
    {
        NetworkHandler _successHandler;
        string _endpoint = "http://server/.well-known/openid-configuration";

        public DiscoveryClientTests()
        {
            var discoFileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "identityserver3.json");
            var document = File.ReadAllText(discoFileName);

            var jwksFileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "identityserver3_jwks.json");
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

        [Theory]
        [InlineData("http://server:123/.well-known/openid-configuration")]
        [InlineData("http://server:123/.well-known/openid-configuration/")]
        [InlineData("http://server:123/")]
        [InlineData("http://server:123")]
        public void various_urls_should_normalize(string input)
        {
            var client = new DiscoveryClient(input);

            client.Url.Should().Be("http://server:123/.well-known/openid-configuration");
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new DiscoveryClient(_endpoint, handler);
            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Http);
            disco.Error.Should().Be("not found");
            disco.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(new Exception("error"));

            var client = new DiscoveryClient(_endpoint, handler);
            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Exception);
            disco.Error.Should().Be("error");
        }

        [Fact]
        public async Task TryGetValue_calls_should_behave_as_excected()
        {
            var client = new DiscoveryClient(_endpoint, _successHandler);
            var disco = await client.GetAsync();

            disco.TryGetValue(OidcConstants.Discovery.AuthorizationEndpoint).Should().NotBeNull();
            disco.TryGetValue("unknown").Should().BeNull();

            disco.TryGetString(OidcConstants.Discovery.AuthorizationEndpoint).Should().Be("https://demo.identityserver.io/connect/authorize");
            disco.TryGetString("unknown").Should().BeNull();
        }

        [Fact]
        public async Task Strongly_typed_accessors_should_behave_as_expected()
        {
            var client = new DiscoveryClient(_endpoint, _successHandler);
            var disco = await client.GetAsync();

            disco.TokenEndpoint.Should().Be("https://demo.identityserver.io/connect/token");
            disco.AuthorizeEndpoint.Should().Be("https://demo.identityserver.io/connect/authorize");
            disco.UserInfoEndpoint.Should().Be("https://demo.identityserver.io/connect/userinfo");

            disco.FrontChannelLogoutSupported.Should().Be(true);
            disco.FrontChannelLogoutSessionSupported.Should().Be(true);

            var responseModes = disco.ResponseModesSupported;

            responseModes.Should().Contain("form_post");
            responseModes.Should().Contain("query");
            responseModes.Should().Contain("fragment");
        }
    }
}