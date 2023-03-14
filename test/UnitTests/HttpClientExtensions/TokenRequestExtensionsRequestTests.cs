// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class TokenRequestExtensionsRequestTests
    {
        private const string Endpoint = "http://server/token";

        private HttpClient _client;
        private NetworkHandler _handler;

        public TokenRequestExtensionsRequestTests()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            _handler = new NetworkHandler(document, HttpStatusCode.OK);

            _client = new HttpClient(_handler)
            {
                BaseAddress = new Uri(Endpoint)
            };
        }

        [Fact]
        public async Task Http_request_should_have_correct_format()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var request = new TokenRequest
            {
                Address = Endpoint,
                ClientId = "client",
                
                GrantType = "grant"
            };

            request.Headers.Add("custom", "custom");
            request.Properties.Add("custom", "custom");

            var _ = await client.RequestTokenAsync(request);
            var httpRequest = handler.Request;

            httpRequest.Method.Should().Be(HttpMethod.Post);
            httpRequest.RequestUri.Should().Be(new Uri(Endpoint));
            httpRequest.Content.Should().NotBeNull();

            var headers = httpRequest.Headers;
            headers.Count().Should().Be(3);
            headers.Should().Contain(h => h.Key == "custom" && h.Value.First() == "custom");

            var properties = httpRequest.Properties;
            properties.Count.Should().Be(1);

            var prop = properties.First();
            prop.Key.Should().Be("custom");
            ((string)prop.Value).Should().Be("custom");
        }

        [Fact]
        public async Task No_explicit_endpoint_address_should_use_base_addess()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                { ClientId = "client" });

            response.IsError.Should().BeFalse();
            _handler.Request.RequestUri.AbsoluteUri.Should().Be(Endpoint);
        }

        [Fact]
        public async Task Repeating_a_request_should_succeed()
        {
            var request = new ClientCredentialsTokenRequest
            {
                ClientId = "client",
                Scope = "scope"
            };

            var response = await _client.RequestClientCredentialsTokenAsync(request);
            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.ClientCredentials);

            fields.TryGetValue("scope", out var scope).Should().BeTrue();
            scope.First().Should().Be("scope");

            response = await _client.RequestClientCredentialsTokenAsync(request);
            response.IsError.Should().BeFalse();

            fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.ClientCredentials);

            fields.TryGetValue("scope", out scope).Should().BeTrue();
            scope.First().Should().Be("scope");
        }

        [Fact]
        public async Task Client_credentials_request_should_have_correct_format()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                ClientId = "client",
                Scope = "scope",
                Resource = { "resource1", "resource2" }
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.ClientCredentials);

            fields.TryGetValue("scope", out var scope).Should().BeTrue();
            scope.First().Should().Be("scope");

            fields.TryGetValue("resource", out var resource).Should().BeTrue();
            resource.Count.Should().Be(2);
            resource[0].Should().Be("resource1");
            resource[1].Should().Be("resource2");
        }

        [Fact]
        public async Task Additional_headers_should_be_propagated()
        {
            var request = new ClientCredentialsTokenRequest
            {
                ClientId = "client",
                Scope = "scope"
            };

            request.Headers.Add("foo", "bar");

            var response = await _client.RequestClientCredentialsTokenAsync(request);

            response.IsError.Should().BeFalse();

            var headers = _handler.Request.Headers;
            var foo = headers.FirstOrDefault(h => h.Key == "foo");
            foo.Should().NotBeNull();
            foo.Value.Single().Should().Be("bar");
        }

        [Fact]
        public async Task Additional_request_properties_should_be_propagated()
        {
            var request = new ClientCredentialsTokenRequest
            {
                ClientId = "client",
                Scope = "scope"
            };

            request.Properties.Add("foo", "bar");

            var response = await _client.RequestClientCredentialsTokenAsync(request);

            response.IsError.Should().BeFalse();

            var properties = _handler.Request.Properties;
            var foo = properties.First().Value as string;
            foo.Should().NotBeNull();
            foo.Should().Be("bar");
        }

        [Fact]
        public async Task dpop_proof_token_should_be_propagated()
        {
            var request = new ClientCredentialsTokenRequest
            {
                ClientId = "client",
                Scope = "scope",
                DPoPProofToken = "dpop_token"
            };

            var response = await _client.RequestClientCredentialsTokenAsync(request);

            response.IsError.Should().BeFalse();
            _handler.Request.Headers.Single(x => x.Key == "DPoP").Value.First().Should().Be("dpop_token");
        }

        [Fact]
        public async Task dpop_nonce_should_be_returned()
        {
            _handler = new NetworkHandler(req =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                resp.Headers.Add("DPoP-Nonce", "dpop_nonce");
                resp.Content = new FormUrlEncodedContent(new Dictionary<string, string>());
                return resp;
            });
            _client = new HttpClient(_handler)
            {
                BaseAddress = new Uri(Endpoint)
            };

            var request = new ClientCredentialsTokenRequest
            {
                ClientId = "client",
                Scope = "scope",
            };
            
            var response = await _client.RequestClientCredentialsTokenAsync(request);

            response.IsError.Should().BeTrue();
            response.DPoPNonce.Should().Be("dpop_nonce");
        }


        [Fact]
        public void Explicit_null_parameters_should_not_fail_()
        {
            Func<Task> act = async () =>
                await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    { ClientId = "client", Parameters = null });

            act.Should().NotThrow();
        }

        [Fact]
        public async Task Device_request_should_have_correct_format()
        {
            var response = await _client.RequestDeviceTokenAsync(new DeviceTokenRequest
            {
                ClientId = "device",
                DeviceCode = "device_code"
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.DeviceCode);

            fields.TryGetValue("device_code", out var device_code).Should().BeTrue();
            device_code.First().Should().Be("device_code");
        }

        [Fact]
        public void Device_request_without_device_code_should_fail()
        {
            Func<Task> act = async () =>
                await _client.RequestDeviceTokenAsync(new DeviceTokenRequest { ClientId = "device" });

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("device_code");
        }

        [Fact]
        public async Task Password_request_should_have_correct_format()
        {
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                ClientId = "client",
                UserName = "user",
                Password = "password",
                Scope = "scope",
                Resource = { "resource1", "resource2" }
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

            fields.TryGetValue("resource", out var resource).Should().BeTrue();
            resource.Count.Should().Be(2);
            resource[0].Should().Be("resource1");
            resource[1].Should().Be("resource2");
        }

        [Fact]
        public async Task Password_request_without_password_should_have_correct_format()
        {
            var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                ClientId = "client",
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
                ClientId = "client",
                Code = "code",
                RedirectUri = "uri",
                CodeVerifier = "verifier",
                Resource = { "resource1", "resource2" },
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

            fields.TryGetValue("resource", out var resource).Should().BeTrue();
            resource.Count.Should().Be(2);
            resource[0].Should().Be("resource1");
            resource[1].Should().Be("resource2");
        }

        [Fact]
        public void Code_request_without_code_should_fail()
        {
            Func<Task> act = async () => await _client.RequestAuthorizationCodeTokenAsync(
                new AuthorizationCodeTokenRequest
                {
                    RedirectUri = "uri"
                });

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("code");
        }

        [Fact]
        public void Code_request_without_redirect_uri_should_fail()
        {
            Func<Task> act = async () => await _client.RequestAuthorizationCodeTokenAsync(
                new AuthorizationCodeTokenRequest
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
                ClientId = "client",
                RefreshToken = "rt",
                Scope = "scope",
                Resource = { "resource1", "resource2" }
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.RefreshToken);

            fields.TryGetValue("refresh_token", out var code).Should().BeTrue();
            code.First().Should().Be("rt");

            fields.TryGetValue("scope", out var redirect_uri).Should().BeTrue();
            redirect_uri.First().Should().Be("scope");

            fields.TryGetValue("resource", out var resource).Should().BeTrue();
            resource.Count.Should().Be(2);
            resource[0].Should().Be("resource1");
            resource[1].Should().Be("resource2");
        }

        [Fact]
        public void Refresh_request_without_refresh_token_should_fail()
        {
            Func<Task> act = async () => await _client.RequestRefreshTokenAsync(new RefreshTokenRequest());

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("refresh_token");
        }

        [Fact]
        public async Task TokenExchange_request_should_have_correct_format()
        {
            var response = await _client.RequestTokenExchangeTokenAsync(new TokenExchangeTokenRequest
            {
                ClientId = "client",

                SubjectToken = "subject_token",
                SubjectTokenType = "subject_token_type",

                Scope = "scope",
                Resource = "resource",
                Audience = "audience",

                ActorToken = "actor_token",
                ActorTokenType = "actor_token_type"
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.TokenExchange);

            fields.TryGetValue("subject_token", out var subject_token).Should().BeTrue();
            subject_token.First().Should().Be("subject_token");

            fields.TryGetValue("subject_token_type", out var subject_token_type).Should().BeTrue();
            subject_token_type.First().Should().Be("subject_token_type");

            fields.TryGetValue("scope", out var scope).Should().BeTrue();
            scope.First().Should().Be("scope");

            fields.TryGetValue("resource", out var resource).Should().BeTrue();
            resource.First().Should().Be("resource");

            fields.TryGetValue("audience", out var audience).Should().BeTrue();
            audience.First().Should().Be("audience");

            fields.TryGetValue("actor_token", out var actor_token).Should().BeTrue();
            actor_token.First().Should().Be("actor_token");

            fields.TryGetValue("actor_token_type", out var actor_token_type).Should().BeTrue();
            actor_token_type.First().Should().Be("actor_token_type");
        }
        
        [Fact]
        public async Task Backchannel_authentication_request_should_have_correct_format()
        {
            var response = await _client.RequestBackchannelAuthenticationTokenAsync(new BackchannelAuthenticationTokenRequest()
            {
                ClientId = "client",
                AuthenticationRequestId = "id",
                
                Resource =
                {
                    "resource1",
                    "resource2"
                }
            });

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.Ciba);

            fields.TryGetValue("auth_req_id", out var id).Should().BeTrue();
            id.First().Should().Be("id");
            
            fields.TryGetValue(OidcConstants.TokenRequest.Resource, out var resource).Should().BeTrue();
            resource.Count.Should().Be(2);
            resource.First().Should().Be("resource1");
            resource.Skip(1).First().Should().Be("resource2");
        }

        [Fact]
        public void Setting_no_grant_type_should_fail()
        {
            Func<Task> act = async () => await _client.RequestTokenAsync(new TokenRequest());

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("grant_type");
        }

        [Fact]
        public async Task Setting_custom_parameters_should_have_correct_format()
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
        public async Task Setting_grant_type_via_optional_parameters_should_create_correct_format()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                ClientId = "client",
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
        public async Task Sending_raw_parameters_should_create_correct_format()
        {
            var response = await _client.RequestTokenRawAsync("https://token/", new Parameters
            {
                { "grant_type", "test" },
                { "client_id", "client" },
                { "client_secret", "secret" },
                { "scope", "scope" }
            });

            var request = _handler.Request;

            request.RequestUri.AbsoluteUri.Should().Be("https://token/");


            var fields = QueryHelpers.ParseQuery(_handler.Body);

            fields.TryGetValue("grant_type", out var field).Should().BeTrue();
            field.First().Should().Be("test");

            fields.TryGetValue("client_id", out field).Should().BeTrue();
            field.First().Should().Be("client");

            fields.TryGetValue("client_secret", out field).Should().BeTrue();
            field.First().Should().Be("secret");

            fields.TryGetValue("scope", out field).Should().BeTrue();
            field.First().Should().Be("scope");
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
            request.Headers.Authorization.Parameter.Should()
                .Be(BasicAuthenticationOAuthHeaderValue.EncodeCredential("client", "secret"));
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
            request.Headers.Authorization.Parameter.Should()
                .Be(BasicAuthenticationOAuthHeaderValue.EncodeCredential("client", ""));

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("client_secret", out _).Should().BeFalse();
            fields.TryGetValue("client_id", out _).Should().BeFalse();
        }

        [Fact]
        public async Task Setting_client_id_and_assertion_with_authorization_header_should_fail()
        {
            Func<Task> act = () => _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                ClientId = "client",
                ClientAssertion = { Type = "type", Value = "value" },
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("CredentialStyle.AuthorizationHeader and client assertions are not compatible");
        }

        [Fact]
        public async Task Setting_client_id_and_assertion_should_have_correct_format()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                ClientId = "client",
                ClientAssertion = { Type = "type", Value = "value" },
                ClientCredentialStyle = ClientCredentialStyle.PostBody
            });

            var request = _handler.Request;

            var fields = QueryHelpers.ParseQuery(_handler.Body);

            fields["grant_type"].First().Should().Be("test");
            fields["client_id"].First().Should().Be("client");
            fields["client_assertion_type"].First().Should().Be("type");
            fields["client_assertion"].First().Should().Be("value");
        }
        
        [Fact]
        public async Task Setting_assertion_without_client_id_and_authz_header_should_have_correct_format()
        {
            var response = await _client.RequestTokenAsync(new TokenRequest
            {
                GrantType = "test",
                ClientAssertion = { Type = "type", Value = "value" },
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            var request = _handler.Request;

            var fields = QueryHelpers.ParseQuery(_handler.Body);

            fields["grant_type"].First().Should().Be("test");
            fields["client_assertion_type"].First().Should().Be("type");
            fields["client_assertion"].First().Should().Be("value");
        }
    }
}