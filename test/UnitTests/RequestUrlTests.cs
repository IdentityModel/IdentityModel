// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class RequestUrlTests
    {
        [Fact]
        public void null_value_should_return_base()
        {
            var request = new RequestUrl("http://server/authorize");

            var url = request.Create(null);

            url.Should().Be("http://server/authorize");
        }

        [Fact]
        public void empty_value_should_return_base()
        {
            var request = new RequestUrl("http://server/authorize");

            var values = new Parameters();
            var url = request.Create(values);

            url.Should().Be("http://server/authorize");
        }

        [Fact]
        public void Create_absolute_url_should_behave_as_expected()
        {
            var request = new RequestUrl("http://server/authorize");

            var parameters = new Parameters
            {
                { "foo", "foo" },
                { "bar", "bar" }
            };

            var url = request.Create(parameters);

            url.Should().Be("http://server/authorize?foo=foo&bar=bar");
        }
        
        [Fact]
        public void Multiple_parameter_names_should_behave_as_expected()
        {
            var request = new RequestUrl("http://server/authorize");

            var parameters = new Parameters
            {
                { "foo", "foo" },
                { "foo", "bar" }
            };

            var url = request.Create(parameters);

            url.Should().Be("http://server/authorize?foo=foo&foo=bar");
        }

        [Fact]
        public void Special_characters_in_query_param_should_be_encoded_correctly()
        {
            var request = new RequestUrl("http://server/authorize");

            var parmeters = new Parameters
            {
                { "scope", "a b c" },
                { "clientId" , "a+b+c" }
            };

            var url = request.Create(parmeters);

            url.Should().Be("http://server/authorize?scope=a%20b%20c&clientId=a%2Bb%2Bc");
        }

        [Fact]
        public void Create_relative_url_should_behave_as_expected()
        {
            var request = new RequestUrl("/authorize");

            var parameters = new Parameters
            {
                { "foo", "foo" },
                { "bar", "bar" }
            };

            var url = request.Create(parameters);

            url.Should().Be("/authorize?foo=foo&bar=bar");
        }

        [Fact]
        public void Null_values_should_be_skipped()
        {
            var request = new RequestUrl("/authorize");

            var parameters = new Parameters
            {
                { "foo", "foo" },
                { "bar", null }
            };

            var url = request.Create(parameters);

            url.Should().Be("/authorize?foo=foo");
        }
    }
}