using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class AuthorizeResponseTests
    {
        [Fact]
        public void Error_Response_with_QueryString()
        {
            var url = "http://server/callback?error=foo";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeTrue();
            response.Error.Should().Be("foo");
        }

        [Fact]
        public void Error_Response_with_HashFragment()
        {
            var url = "http://server/callback#error=foo";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeTrue();
            response.Error.Should().Be("foo");
        }

        [Fact]
        public void Error_Response_with_QueryString_and_HashFragment()
        {
            var url = "http://server/callback?error=foo#_=_";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeTrue();
            response.Error.Should().Be("foo");
        }

        [Fact]
        public void Code_Response_with_QueryString()
        {
            var url = "http://server/callback?code=foo&sid=123";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeFalse();
            response.Code.Should().Be("foo");

            response.Values["sid"].Should().Be("123");
            response.TryGet("sid").Should().Be("123");
        }

        [Fact]
        public void AccessToken_Response_with_QueryString()
        {
            var url = "http://server/callback#access_token=foo&sid=123";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeFalse();
            response.AccessToken.Should().Be("foo");

            response.Values["sid"].Should().Be("123");
            response.TryGet("sid").Should().Be("123");
        }

        [Fact]
        public void AccessToken_Response_with_QueryString_and_HashFragment()
        {
            var url = "http://server/callback?access_token=foo&sid=123#_=_";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeFalse();
            response.AccessToken.Should().Be("foo");

            response.Values["sid"].Should().Be("123");
            response.TryGet("sid").Should().Be("123");
        }
    }
}