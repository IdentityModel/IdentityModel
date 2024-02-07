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
        public void Create_basic_oidc_authorize_url_should_succeed()
        {
            var request = new RequestUrl(Authority);

            var urlString = request.CreateAuthorizeUrl(new AuthorizeRedirectParameters(ClientId, ResponseType)
            {
                Scope = "openid profile",
                RedirectUri = "https://app1.example.com/signin-oidc",
                State = "state",
                Nonce = "nonce",
                CodeChallenge = "challenge",
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Sha256
            });
            var url = new Uri(urlString);

            url.GetLeftPart(UriPartial.Path).Should().Be(Authority); 

            var fields = QueryHelpers.ParseQuery(url.Query);
            fields.Count.Should().Be(8);
            fields["client_id"].Single().Should().Be(ClientId);
            fields["response_type"].Single().Should().Be(ResponseType);
            fields["scope"].Single().Should().Be("openid profile");
            fields["redirect_uri"].Single().Should().Be("https://app1.example.com/signin-oidc");
            fields["state"].Single().Should().Be("state");
            fields["nonce"].Single().Should().Be("nonce");
            fields["code_challenge"].Single().Should().Be("challenge");
            fields["code_challenge_method"].Single().Should().Be(OidcConstants.CodeChallengeMethods.Sha256);
        }


        [Fact]
        public void Create_basic_oauth_authorize_url_should_succeed()
        {
            var request = new RequestUrl(Authority);

            var urlString = request.CreateAuthorizeUrl(new AuthorizeRedirectParameters(ClientId, ResponseType)
            {
                Scope = "api1",
                RedirectUri = "https://app1.example.com/signin-oidc",
                CodeChallenge = "challenge",
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Sha256
            });
            var url = new Uri(urlString);
            
            url.GetLeftPart(UriPartial.Path).Should().Be(Authority); 

            var fields = QueryHelpers.ParseQuery(url.Query);
            fields.Count.Should().Be(6);
            fields["client_id"].Single().Should().Be(ClientId);
            fields["response_type"].Single().Should().Be(ResponseType);
            fields["scope"].Single().Should().Be("api1");
            fields["redirect_uri"].Single().Should().Be("https://app1.example.com/signin-oidc");
            fields["code_challenge"].Single().Should().Be("challenge");
            fields["code_challenge_method"].Single().Should().Be(OidcConstants.CodeChallengeMethods.Sha256);
        }

        [Fact]
        public void Create_oauth_authorize_url_with_resources_should_succeed()
        {
            var request = new RequestUrl(Authority);
            var urlString = request.CreateAuthorizeUrl(new AuthorizeRedirectParameters(ClientId, ResponseType)

            {
                Scope = "api1.read-only api2.admin",
                RedirectUri = "https://app1.example.com/signin-oidc",
                CodeChallenge = "challenge",
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Sha256,
                Resource = new List<string> { "urn:api1", "urn:api2" }
            });
            var url = new Uri(urlString);

            url.GetLeftPart(UriPartial.Path).Should().Be(Authority); 

            var fields = QueryHelpers.ParseQuery(url.Query);
            fields.Count.Should().Be(7);
            fields["client_id"].Single().Should().Be(ClientId);
            fields["response_type"].Single().Should().Be(ResponseType);
            fields["scope"].Single().Should().Be("api1.read-only api2.admin");
            fields["redirect_uri"].Single().Should().Be("https://app1.example.com/signin-oidc");
            fields["code_challenge"].Single().Should().Be("challenge");
            fields["code_challenge_method"].Single().Should().Be(OidcConstants.CodeChallengeMethods.Sha256);
            var resources = fields["resource"];
            resources.Count().Should().Be(2);
            resources.ToList().Should()
                .Contain("urn:api1").And
                .Contain("urn:api2");
        }

        [Fact]
        public void Create_authorize_url_with_all_parameters_should_succeed()
        {
            var request = new RequestUrl(Authority);
            var urlString = request.CreateAuthorizeUrl(new AuthorizeRedirectParameters(ClientId, ResponseType)

            {
                Scope = "api1.read-only api2.admin",
                RedirectUri = "https://app1.example.com/signin-oidc",
                State = "state",
                Nonce = "nonce",
                LoginHint = "loginHint",
                AcrValues = "acrValues",
                Prompt = "prompt",
                ResponseMode = "responseMode",
                CodeChallenge = "challenge",
                CodeChallengeMethod = OidcConstants.CodeChallengeMethods.Sha256,
                Display = "display",
                MaxAge = 100,
                UiLocales = "uiLocales",
                IdTokenHint = "idTokenHint",
                Resource = new List<string> { "urn:api1", "urn:api2" },
                DPoPKeyThumbprint = "dPoPKeyThumbprint",
            });
            var url = new Uri(urlString);

            url.GetLeftPart(UriPartial.Path).Should().Be(Authority); 

            var fields = QueryHelpers.ParseQuery(url.Query);
            fields.Count.Should().Be(18);
            fields["client_id"].Single().Should().Be(ClientId);
            fields["response_type"].Single().Should().Be(ResponseType);
            fields["scope"].Single().Should().Be("api1.read-only api2.admin");
            fields["redirect_uri"].Single().Should().Be("https://app1.example.com/signin-oidc");
            fields["state"].Single().Should().Be("state");
            fields["nonce"].Single().Should().Be("nonce");
            fields["login_hint"].Single().Should().Be("loginHint");
            fields["acr_values"].Single().Should().Be("acrValues");
            fields["prompt"].Single().Should().Be("prompt");
            fields["response_mode"].Single().Should().Be("responseMode");
            fields["code_challenge"].Single().Should().Be("challenge");
            fields["code_challenge_method"].Single().Should().Be(OidcConstants.CodeChallengeMethods.Sha256);
            var resources = fields["resource"];
            resources.Count().Should().Be(2);
            resources.ToList().Should()
                .Contain("urn:api1").And
                .Contain("urn:api2");
            fields["dpop_jkt"].Single().Should().Be("dPoPKeyThumbprint");
            fields["display"].Single().Should().Be("display");
            fields["max_age"].Single().Should().Be("100");
            fields["ui_locales"].Single().Should().Be("uiLocales");
            fields["id_token_hint"].Single().Should().Be("idTokenHint");
        }

        [Fact]
        public void Create_authorize_url_for_par_should_succeed()
        {
            var request = new RequestUrl(Authority);

            var urlString = request.CreateAuthorizeUrl(new RequestUriRedirectParameters(ClientId, "urn:ietf:params:oauth:request_uri:1234"));
            var url = new Uri(urlString);
            
            url.GetLeftPart(UriPartial.Path).Should().Be(Authority); 

            var fields = QueryHelpers.ParseQuery(url.Query);
            fields.Count.Should().Be(2);
            fields["client_id"].Single().Should().Be(ClientId);
            fields["request_uri"].Single().Should().Be("urn:ietf:params:oauth:request_uri:1234");
        }

        [Fact]
        public void Create_authorize_url_for_jar_should_succeed()
        {
            var request = new RequestUrl(Authority);

            var urlString = request.CreateAuthorizeUrl(new RequestObjectRedirectParameters(ClientId, "request jwt"));
            var url = new Uri(urlString);
            
            url.GetLeftPart(UriPartial.Path).Should().Be(Authority); 

            var fields = QueryHelpers.ParseQuery(url.Query);
            fields.Count.Should().Be(2);
            fields["client_id"].Single().Should().Be(ClientId);
            fields["request"].Single().Should().Be("request jwt");
        }
    }
}