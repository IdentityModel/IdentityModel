using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class TokenClientRequestTests
    {
        private const string Endpoint = "http://server/token";

        private readonly HttpClient _client;
        private readonly NetworkHandler _handler;

        public TokenClientRequestTests()
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
            var tokenClient = new TokenClient(_client, new TokenClientOptions { ClientId = "client" });

            var response = await tokenClient.RequestClientCredentialsTokenAsync();

            response.IsError.Should().BeFalse();
            _handler.Request.RequestUri.AbsoluteUri.Should().Be(Endpoint);
        }

        [Fact]
        public async Task Client_credentials_request_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions { ClientId = "client" });

            var response = await tokenClient.RequestClientCredentialsTokenAsync(scope: "scope");

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.ClientCredentials);

            fields.TryGetValue("scope", out var scope).Should().BeTrue();
            scope.First().Should().Be("scope");
        }

        [Fact]
        public async Task Device_request_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions { ClientId = "device" });

            var response = await tokenClient.RequestDeviceTokenAsync(deviceCode: "device_code");

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.DeviceCode);

            fields.TryGetValue("client_id", out var client_id).Should().BeTrue();
            client_id.First().Should().Be("device");

            fields.TryGetValue("device_code", out var device_code).Should().BeTrue();
            device_code.First().Should().Be("device_code");
        }

        [Fact]
        public void Device_request_without_device_code_should_fail()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions { ClientId = "device" });

            Func<Task> act = async () => await tokenClient.RequestDeviceTokenAsync(null);

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("device_code");
        }

        [Fact]
        public async Task Password_request_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions { ClientId = "client" });

            var response = await tokenClient.RequestPasswordTokenAsync(userName: "user", password: "password", scope: "scope");

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("grant_type", out var grant_type).Should().BeTrue();
            grant_type.First().Should().Be(OidcConstants.GrantTypes.Password);

            fields.TryGetValue("username", out var username).Should().BeTrue();
            username.First().Should().Be("user");

            fields.TryGetValue("password", out var password).Should().BeTrue();
            password.First().Should().Be("password");

            fields.TryGetValue("scope", out var scope).Should().BeTrue();
            scope.First().Should().Be("scope");
        }

        [Fact]
        public async Task Password_request_without_password_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions { ClientId = "client" });

            var response = await tokenClient.RequestPasswordTokenAsync(userName: "user");

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
            var tokenClient = new TokenClient(_client, new TokenClientOptions());

            Func<Task> act = async () => await tokenClient.RequestPasswordTokenAsync(userName: null);

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("username");
        }

        [Fact]
        public async Task Code_request_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions { ClientId = "client" });

            var response = await tokenClient.RequestAuthorizationCodeTokenAsync(code: "code", redirectUri: "uri", codeVerifier: "verifier");

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
            var tokenClient = new TokenClient(_client, new TokenClientOptions());

            Func<Task> act = async () => await tokenClient.RequestAuthorizationCodeTokenAsync(code: null, redirectUri: "uri", codeVerifier: "verifier");

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("code");
        }

        [Fact]
        public void Code_request_without_redirect_uri_should_fail()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions());

            Func<Task> act = async () => await tokenClient.RequestAuthorizationCodeTokenAsync(code: "code", redirectUri: null, codeVerifier: "verifier");

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("redirect_uri");
        }

        [Fact]
        public async Task Refresh_request_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions { ClientId = "client" });

            var response = await tokenClient.RequestRefreshTokenAsync(refreshToken: "rt", scope: "scope");

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
        public void Refresh_request_without_refresh_token_should_fail()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions());

            Func<Task> act = async () => await tokenClient.RequestRefreshTokenAsync(refreshToken: null, scope: "scope");

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("refresh_token");
        }

        [Fact]
        public void Setting_no_grant_type_should_fail()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions());

            Func<Task> act = async () => await tokenClient.RequestTokenAsync(grantType: null);

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("grant_type");
        }

        [Fact]
        public async Task Setting_custom_parameters_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions());

            var parameters = new Parameters
            {
                { "client_id", "custom" },
                { "client_secret", "custom" },
                { "custom", "custom" }
            };

            var response = await tokenClient.RequestTokenAsync(grantType: "test", parameters: parameters);

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
        public async Task Mixing_local_and_global_custom_parameters_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions { Parameters = { { "global", "global" } } });

            var parameters = new Parameters
            {
                { "client_id", "custom" },
                { "client_secret", "custom" },
                { "custom", "custom" }
            };

            var response = await tokenClient.RequestTokenAsync(grantType: "test", parameters: parameters);

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

            fields.TryGetValue("global", out var global).Should().BeTrue();
            global.First().Should().Be("global");
        }

        [Fact]
        public async Task Local_custom_parameters_should_not_interfere_with_global()
        {
            var globalOptions = new TokenClientOptions { Parameters = { { "global", "value" } } };
            var tokenClient = new TokenClient(_client, globalOptions);

            var localParameters = new Parameters
            {
                { "client_id", "custom" },
                { "client_secret", "custom" },
                { "custom", "custom" }
            };
            
            _ = await tokenClient.RequestTokenAsync(grantType: "test", parameters: localParameters);

            globalOptions.Parameters.Should().HaveCount(1);
            var globalValue = globalOptions.Parameters.FirstOrDefault(p => p.Key == "global").Value;
            globalValue.Should().Be("value");
        }

        [Fact]
        public async Task Setting_basic_authentication_style_should_send_basic_authentication_header()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions
            {
                ClientId = "client",
                ClientSecret = "secret",
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            var response = await tokenClient.RequestTokenAsync(grantType: "test");

            var request = _handler.Request;

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.Scheme.Should().Be("Basic");
            request.Headers.Authorization.Parameter.Should().Be(BasicAuthenticationOAuthHeaderValue.EncodeCredential("client", "secret"));
        }

        [Fact]
        public async Task Setting_post_values_authentication_style_should_post_values()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions
            {
                ClientId = "client",
                ClientSecret = "secret",
                ClientCredentialStyle = ClientCredentialStyle.PostBody
            });

            var response = await tokenClient.RequestTokenAsync(grantType: "test");

            var request = _handler.Request;
            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields["client_id"].First().Should().Be("client");
            fields["client_secret"].First().Should().Be("secret");

        }

        [Fact]
        public async Task Setting_client_id_only_and_post_should_put_client_id_in_post_body()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions
            {
                ClientId = "client",
                ClientCredentialStyle = ClientCredentialStyle.PostBody
            });

            var response = await tokenClient.RequestTokenAsync(grantType: "test");

            var request = _handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields["client_id"].First().Should().Be("client");
        }

        [Fact]
        public async Task Setting_client_id_only_and_header_should_put_client_id_in_header()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions
            {
                ClientId = "client",
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            var response = await tokenClient.RequestTokenAsync(grantType: "test");

            var request = _handler.Request;

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.Scheme.Should().Be("Basic");
            request.Headers.Authorization.Parameter.Should().Be(BasicAuthenticationOAuthHeaderValue.EncodeCredential("client", ""));

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields.TryGetValue("client_secret", out _).Should().BeFalse();
            fields.TryGetValue("client_id", out _).Should().BeFalse();
        }

        [Fact]
        public async Task Setting_client_id_and_assertion_should_have_correct_format()
        {
            var tokenClient = new TokenClient(_client, new TokenClientOptions
            {
                ClientId = "client",
                ClientAssertion = { Type = "type", Value = "value" }
            });

            var response = await tokenClient.RequestTokenAsync(grantType: "test");
            var fields = QueryHelpers.ParseQuery(_handler.Body);

            fields["grant_type"].First().Should().Be("test");
            fields["client_id"].First().Should().Be("client");
            fields["client_assertion_type"].First().Should().Be("type");
            fields["client_assertion"].First().Should().Be("value");
        }
    }
}
