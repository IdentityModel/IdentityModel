// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class DiscoveryPolicyTests_WithoutAuthorityValidation
    {
        [Theory]
        [InlineData("foo")]
        [InlineData("file://some_file")]
        [InlineData("https:something_weird_https://something_other")]
        public void Malformed_authority_url_should_throw(string input)
        {
            Action act = () => DiscoveryEndpoint.ParseUrl(input);

            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.Equals("Malformed URL"));
        }

        [Theory]
        [InlineData("https://server:123/.well-known/openid-configuration")]
        [InlineData("https://server:123/.well-known/openid-configuration/")]
        [InlineData("https://server:123/")]
        [InlineData("https://server:123")]
        public void Various_urls_should_normalize(string input)
        {
            var result = DiscoveryEndpoint.ParseUrl(input);

            // test parse URL logic
            result.Url.Should().Be("https://server:123/.well-known/openid-configuration");
            result.Authority.Should().Be("https://server:123");
        }

        [Theory]
        [InlineData("https://server:123/strange-location/openid-configuration", "/strange-location/openid-configuration")]
        [InlineData("https://server:123/strange-location/openid-configuration", "strange-location/openid-configuration")]
        [InlineData("https://server:123/strange-location/openid-configuration/", "/strange-location/openid-configuration")]
        [InlineData("https://server:123/", "/strange-location/openid-configuration")]
        [InlineData("https://server:123", "/strange-location/openid-configuration")]
        public void Custom_path_is_supported(string input, string documentPath)
        {
            var result = DiscoveryEndpoint.ParseUrl(input, documentPath);

            // test parse URL logic
            result.Url.Should().Be("https://server:123/strange-location/openid-configuration");
            result.Authority.Should().Be("https://server:123");
        }
    }
}