// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class RequestUrlTests
    {

        private const string ClientId = "client";
        private const string ResponseType = "code";
        private const string Authority = "http://server/authorize";
        

        [Fact]
        public void null_value_should_return_base()
        {
            var request = new RequestUrl(Authority);

            var url = request.Create(null);

            url.Should().Be(Authority);
        }

        [Fact]
        public void empty_value_should_return_base()
        {
            var request = new RequestUrl(Authority);

            var values = new Parameters();
            var url = request.Create(values);

            url.Should().Be(Authority);
        }

        [Fact]
        public void Create_absolute_url_should_behave_as_expected()
        {
            var request = new RequestUrl(Authority);

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
            var request = new RequestUrl(Authority);

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
            var request = new RequestUrl(Authority);

            var parameters = new Parameters
            {
                { "scope", "a b c" },
                { "clientId" , "a+b+c" }
            };

            var url = request.Create(parameters);

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

        [Fact]
        public void Create_authorize_url_for_par_should_succeed()
        {
            var request = new RequestUrl(Authority);
            var requestUri = "urn:ietf:params:oauth:request_uri:1234";

            var urlString = request.CreateAuthorizeUrl(clientId: ClientId, requestUri: requestUri);
            var url = new Uri(urlString);
            
            url.GetLeftPart(UriPartial.Path).Should().Be(Authority); 

            var fields = QueryHelpers.ParseQuery(url.Query);
            fields.Count.Should().Be(2);
            fields["client_id"].Single().Should().Be(ClientId);
            fields["request_uri"].Single().Should().Be(requestUri);
        }
        
        [Fact]
        public void Create_authorize_url_for_non_par_should_succeed()
        {
            var request = new RequestUrl(Authority);
            var redirectUri = "https://app.example.com/signin-oidc";
            var scope = "api1";
            var state = "state";

            var urlString = request.CreateAuthorizeUrl(
                clientId: ClientId, 
                responseType: "code",
                redirectUri: redirectUri,
                scope: scope,
                state: state
            );
            var url = new Uri(urlString);
            
            url.GetLeftPart(UriPartial.Path).Should().Be(Authority); 

            var fields = QueryHelpers.ParseQuery(url.Query);
            fields.Count.Should().Be(5);
            fields["client_id"].Single().Should().Be(ClientId);
            fields["response_type"].Single().Should().Be("code");
            fields["redirect_uri"].Single().Should().Be(redirectUri);
            fields["scope"].Single().Should().Be(scope);
            fields["state"].Single().Should().Be(state);
        }
    }
}