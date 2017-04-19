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
        string _endpoint = "https://demo.identityserver.io/.well-known/openid-configuration";

        public DiscoveryClientTests()
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

        [Theory]
        [InlineData("foo")]
        [InlineData("file://some_file")]
        [InlineData("https:something_weird_https://something_other")]
        public void malformed_authority_url_should_throw(string input)
        {
            Action act = () => DiscoveryClient.ParseUrl(input);

            act.ShouldThrow<InvalidOperationException>().Where(e => e.Message.Equals("Malformed URL"));
        }

        [Theory]
        [InlineData("https://server:123/.well-known/openid-configuration")]
        [InlineData("https://server:123/.well-known/openid-configuration/")]
        [InlineData("https://server:123/")]
        [InlineData("https://server:123")]
        public void various_urls_should_normalize(string input)
        {
            var result = DiscoveryClient.ParseUrl(input);

            // test parse URL logic
            result.discoveryEndpoint.Should().Be("https://server:123/.well-known/openid-configuration");
            result.authority.Should().Be("https://server:123");

            // make sure parse URL results are used correctly
            var client = new DiscoveryClient(input);
            client.Url.Should().Be(result.discoveryEndpoint);
            client.Authority.Should().Be(result.authority);
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new DiscoveryClient(_endpoint, handler);
            var disco = await client.GetAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Http);
            disco.Error.Should().StartWith("Error connecting to");
            disco.Error.Should().EndWith("not found");
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
            disco.Error.Should().StartWith("Error connecting to");
            disco.Error.Should().EndWith("error");
        }

        [Fact]
        public async Task TryGetValue_calls_should_behave_as_excected()
        {
            var client = new DiscoveryClient(_endpoint, _successHandler);
            var disco = await client.GetAsync();

            disco.IsError.Should().BeFalse();

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

            disco.IsError.Should().BeFalse();

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