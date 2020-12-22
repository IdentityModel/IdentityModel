// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
            const string url = "http://server/callback?error=foo";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeTrue();
            response.Error.Should().Be("foo");
        }

        [Fact]
        public void Error_Response_with_HashFragment()
        {
            const string url = "http://server/callback#error=foo";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeTrue();
            response.Error.Should().Be("foo");
        }

        [Fact]
        public void Error_Response_with_QueryString_and_HashFragment()
        {
            const string url = "http://server/callback?error=foo#_=_";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeTrue();
            response.Error.Should().Be("foo");
        }

        [Fact]
        public void Code_Response_with_QueryString()
        {
            const string url = "http://server/callback?code=foo&sid=123";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeFalse();
            response.Code.Should().Be("foo");

            response.Values["sid"].Should().Be("123");
            response.TryGet("sid").Should().Be("123");
        }

        [Fact]
        public void AccessToken_Response_with_QueryString()
        {
            const string url = "http://server/callback#access_token=foo&sid=123";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeFalse();
            response.AccessToken.Should().Be("foo");

            response.Values["sid"].Should().Be("123");
            response.TryGet("sid").Should().Be("123");
        }

        [Fact]
        public void AccessToken_Response_with_QueryString_and_HashFragment()
        {
            const string url = "http://server/callback?access_token=foo&sid=123#_=_";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeFalse();
            response.AccessToken.Should().Be("foo");

            response.Values["sid"].Should().Be("123");
            response.TryGet("sid").Should().Be("123");
        }

        [Fact]
        public void AccessToken_Response_with_QueryString_and_Empty_Entry()
        {
            const string url = "http://server/callback?access_token=foo&&sid=123&";

            var response = new AuthorizeResponse(url);

            response.IsError.Should().BeFalse();
            response.AccessToken.Should().Be("foo");

            response.Values["sid"].Should().Be("123");
            response.TryGet("sid").Should().Be("123");
        }

        [Fact]
        public void form_post_format_should_parse()
        {
            const string form = "id_token=foo&code=bar&scope=baz&session_state=quux";
            var response = new AuthorizeResponse(form);

            response.IsError.Should().BeFalse();
            response.IdentityToken.Should().Be("foo");
            response.Code.Should().Be("bar");
            response.Scope.Should().Be("baz");
            response.Values["session_state"].Should().Be("quux");
        }
    }
}