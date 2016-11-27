using System;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class AuthorizeRequestTests
    {
        [Fact]
        public void Create_absolute_url_should_behave_as_expected()
        {
            var request = new AuthorizeRequest("http://server/authorize");

            var parmeters = new
            {
                foo = "foo",
                bar = "bar"
            };

            var url = request.Create(parmeters);

            url.Should().Be("http://server/authorize?foo=foo&bar=bar");
        }

        [Fact]
        public void Create_relative_url_should_behave_as_expected()
        {
            var request = new AuthorizeRequest(new Uri("/authorize", UriKind.Relative));

            var parmeters = new
            {
                foo = "foo",
                bar = "bar"
            };

            var url = request.Create(parmeters);

            url.Should().Be("/authorize?foo=foo&bar=bar");
        }
    }
}