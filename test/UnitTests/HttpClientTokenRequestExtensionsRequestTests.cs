// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using IdentityModel.HttpClientExtensions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class HttpClientTokenRequestExtensionsRequestTests
    {
        const string Endpoint = "http://server/token";

        HttpClient _client;
        NetworkHandler _handler;

        public HttpClientTokenRequestExtensionsRequestTests()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            _handler = new NetworkHandler(document, HttpStatusCode.OK);

            _client = new HttpClient(_handler)
            {
                BaseAddress = new Uri(Endpoint)
            };
        }

        [Fact]
        public async Task No_explicit_endpoint_address_should_use_base_addess()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest());
            
            response.IsError.Should().BeFalse();
            _handler.Request.RequestUri.AbsoluteUri.Should().Be(Endpoint);
        }

        [Fact]
        public async Task Client_credentials_request_should_have_correct_format()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Scope = "scope"
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.ClientCredentials);

            fields.TryGetValue("scope", out var scope).Should().BeTrue();
            scope.First().Should().Be("scope");
        }

        [Fact]
        public async Task Password_request_should_have_correct_format()
        {
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                UserName = "user",
                Password = "password",
                Scope = "scope"
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.Password);

            fields.TryGetValue("username", out var username).Should().BeTrue();
            username.First().Should().Be("user");

            fields.TryGetValue("password", out var password).Should().BeTrue();
            grant_type.First().Should().Be("password");

            fields.TryGetValue("scope", out var scope).Should().BeTrue();
            scope.First().Should().Be("scope");
        }

        [Fact]
        public async Task Password_request_without_password_should_have_correct_format()
        {
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                UserName = "user"
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.Password);

            fields.TryGetValue("username", out var username).Should().BeTrue();
            username.First().Should().Be("user");

            fields.TryGetValue("password", out var password).Should().BeTrue();
            password.First().Should().Be("");
        }

        [Fact]
        public void Password_request_without_username_should_fail()
        {
           Func<Task> act = async () => await _client.RequestPasswordTokenAsync(new PasswordTokenRequest());

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("username");
        }

        [Fact]
        public async Task Code_request_should_have_correct_format()
        {
            var response = await _client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Code = "code",
                RedirectUri = "uri",
                CodeVerifier = "verifier"
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.AuthorizationCode);

            fields.TryGetValue("code", out var code).Should().BeTrue();
            code.First().Should().Be("code");

            fields.TryGetValue("redirect_uri", out var redirect_uri).Should().BeTrue();
            redirect_uri.First().Should().Be("uri");

            fields.TryGetValue("code_verifier", out var code_verifier).Should().BeTrue();
            code_verifier.First().Should().Be("verifier");
        }

        [Fact]
        public void Code_request_without_code_should_fail()
        {
            Func<Task> act = async () => await _client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                RedirectUri = "uri"
            });

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("code");
        }

        [Fact]
        public void Code_request_without_redirect_uri_should_fail()
        {
            Func<Task> act = async () => await _client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Code = "code"
            });

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("redirect_uri");
        }

        [Fact]
        public async Task Refresh_request_should_have_correct_format()
        {
            var response = await _client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                RefreshToken = "rt",
                Scope = "scope"
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.RefreshToken);

            fields.TryGetValue("refresh_token", out var code).Should().BeTrue();
            code.First().Should().Be("rt");

            fields.TryGetValue("scope", out var redirect_uri).Should().BeTrue();
            redirect_uri.First().Should().Be("scope");
        }

        [Fact]
        public async Task Assertion_request_should_have_correct_format()
        {
            var response = await _client.RequestAssertionTokenAsync(new AssertionTokenRequest
            {
                AssertionType = "at",
                Assertion = "assertion",
                Scope = "scope"
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be("at");

            fields.TryGetValue("assertion", out var assertion).Should().BeTrue();
            assertion.First().Should().Be("assertion");

            fields.TryGetValue("scope", out var redirect_uri).Should().BeTrue();
            redirect_uri.First().Should().Be("scope");
        }

        [Fact]
        public void Refresh_request_without_refresh_token_should_fail()
        {
            Func<Task> act = async () => await _client.RequestRefreshTokenAsync(new RefreshTokenRequest());

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("refresh_token");
        }

        [Fact]
        public void Setting_no_grant_type_should_fail()
        {
            Func<Task> act = async () => await _client.RequestTokenAsync(new TokenRequest());

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("grant_type");
        }

        [Fact]
        public async Task Setting_custom_parameters_should_have_correct()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                Parameters =
                {
                    { "client_id", "custom" },
                    { "client_secret", "custom" },
                    { "custom", "custom" }
                }
            });

            var request = _handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be("test");

            fields.TryGetValue("client_id", out var client_id).Should().BeTrue();
            client_id.First().Should().Be("custom");

            fields.TryGetValue("client_secret", out var client_secret).Should().BeTrue();
            client_secret.First().Should().Be("custom");

            fields.TryGetValue("custom", out var custom).Should().BeTrue();
            custom.First().Should().Be("custom");
        }

        [Fact]
        public async Task Setting_grant_type_via_optional_parameters_should_created_correct_format()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                Parameters =
                {
                    { "grant_type", "custom" },
                    { "custom", "custom" }
                }
            });

            var request = _handler.Request;

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be("custom");

            fields.TryGetValue("custom", out var custom).Should().BeTrue();
            custom.First().Should().Be("custom");
        }


        [Fact]
        public async Task Setting_basic_authentication_style_should_send_basic_authentication_header()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                ClientId = "client",
                ClientSecret = "secret",
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            var request = _handler.Request;

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.Scheme.Should().Be("Basic");
            request.Headers.Authorization.Parameter.Should().Be(BasicAuthenticationOAuthHeaderValue.EncodeCredential("client", "secret"));
        }

        [Fact]
        public async Task Setting_post_values_authentication_style_should_post_values()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                ClientId = "client",
                ClientSecret = "secret",
                ClientCredentialStyle = ClientCredentialStyle.PostBody
            });

            var request = _handler.Request;
            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields["client_id"].First().Should().Be("client");
            fields["client_secret"].First().Should().Be("secret");

        }

        [Fact]
        public async Task Setting_no_client_id_and_secret_should_not_send_credentials()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                Address = Endpoint,
                GrantType = "test",
            });

            var request = _handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("client_secret", out _).Should().BeFalse();
            fields.TryGetValue("client_id", out _).Should().BeFalse();
        }

        [Fact]
        public async Task Setting_client_id_only_and_post_should_put_client_id_in_post_body()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                ClientId = "client",
                ClientCredentialStyle = ClientCredentialStyle.PostBody
            });

            var request = _handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields["client_id"].First().Should().Be("client");
        }

        [Fact]
        public async Task Setting_client_id_only_and_header_should_put_client_id_in_header()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                ClientId = "client",
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            var request = _handler.Request;

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.Scheme.Should().Be("Basic");
            request.Headers.Authorization.Parameter.Should().Be(BasicAuthenticationOAuthHeaderValue.EncodeCredential("client", ""));

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("client_secret", out _).Should().BeFalse();
            fields.TryGetValue("client_id", out _).Should().BeFalse();
        }
    }
}