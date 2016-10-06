using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class DiscoveryClientTests
    {
        [Theory]
        [InlineData("http://server:123/.well-known/openid-configuration")]
        [InlineData("http://server:123/.well-known/openid-configuration/")]
        [InlineData("http://server:123/")]
        [InlineData("http://server:123")]
        public void VariousUrls(string input)
        {
            var client = new DiscoveryClient(input);

            client.Url.Should().Be("http://server:123/.well-known/openid-configuration");
        }
    }
}
