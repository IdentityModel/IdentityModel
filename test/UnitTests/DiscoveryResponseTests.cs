using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;
using IdentityModel.Client;
using FluentAssertions;

namespace IdentityModel.UnitTests
{
    public class DiscoveryResponseTests
    {
        string _document;

        public DiscoveryResponseTests()
        {
            var fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "identityserver3.json");
            _document = File.ReadAllText(fileName);
        }

        [Fact]
        public void ReadValues()
        {
            var disco = new DiscoveryResponse(_document);

            disco.TryGetValue(OidcConstants.Discovery.AuthorizationEndpoint).Should().NotBeNull();
            disco.TryGetValue("unknown").Should().BeNull();

            disco.TryGetString(OidcConstants.Discovery.AuthorizationEndpoint).Should().Be("https://demo.identityserver.io/connect/authorize");
            disco.TryGetString("unknown").Should().BeNull();
        }

        [Fact]
        public void ReadStrings()
        {
            var disco = new DiscoveryResponse(_document);

            disco.TokenEndpoint.Should().Be("https://demo.identityserver.io/connect/token");
            disco.AuthorizationEndpoint.Should().Be("https://demo.identityserver.io/connect/authorize");
            disco.UserInfoEndpoint.Should().Be("https://demo.identityserver.io/connect/userinfo");
        }

        [Fact]
        public void ReadBooleans()
        {
            var disco = new DiscoveryResponse(_document);

            disco.FrontChannelLogoutSupported.Should().Be(true);
            disco.FrontChannelLogoutSessionSupported.Should().Be(true);
        }

        [Fact]
        public void ReadStringsArrays()
        {
            var disco = new DiscoveryResponse(_document);
            var responseModes = disco.ResponseModesSupported;

            responseModes.Should().Contain("form_post");
            responseModes.Should().Contain("query");
            responseModes.Should().Contain("fragment");
        }
    }
}