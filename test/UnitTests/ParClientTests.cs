using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
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
    public class ParClientTests
    {
        private const string Endpoint = "http://server/par";

        private readonly HttpClient _client;
        private readonly NetworkHandler _handler;

        public ParClientTests()
        {
            var document = File.ReadAllText(FileName.Create("success_par_response.json"));
            _handler = new NetworkHandler(document, HttpStatusCode.OK);

            _client = new HttpClient(_handler)
            {
                BaseAddress = new Uri(Endpoint)
            };
        }

        [Fact]
        public async Task No_explicit_endpoint_address_should_use_base_address()
        {
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions { ClientId = "client" });

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(new Parameters());

            response.IsError.Should().BeFalse();
            _handler.Request.RequestUri.AbsoluteUri.Should().Be(Endpoint);
        }

        [Fact]
        public async Task Pushed_request_should_have_correct_format()
        {
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions { ClientId = "client" });

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(new Parameters(new Dictionary<string, string>
            {
                { "redirect_uri", "https://example.com/signin-oidc" },
                { "response_type", "code" },
                { "nonce", "1234" },
                { "scope", "openid profile" },
                { "state", "5678" },
                { "code_challenge", "98765"}
            }));

            response.IsError.Should().BeFalse();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            
            ValidateRequestParameter(fields, "redirect_uri", "https://example.com/signin-oidc");
            ValidateRequestParameter(fields, "client_id", "client");
            ValidateRequestParameter(fields, "nonce", "1234");
            ValidateRequestParameter(fields, "scope", "openid profile");
            ValidateRequestParameter(fields, "state", "5678");
            ValidateRequestParameter(fields, "code_challenge", "98765");
        }

        [Fact]
        public async Task Setting_custom_parameters_should_have_correct_format()
        {
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions());

            var parameters = new Parameters
            {
                { "client_id", "custom" },
                { "custom", "custom" }
            };

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(parameters: parameters);

            var request = _handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            ValidateRequestParameter(fields, "client_id", "custom");
            ValidateRequestParameter(fields, "custom", "custom");
        }

        [Fact]
        public async Task Mixing_local_and_global_custom_parameters_should_have_correct_format()
        {
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions { Parameters = { { "global", "global" } } });

            var parameters = new Parameters
            {
                { "client_id", "custom" },
                { "custom", "custom" }
            };

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(parameters: parameters);

            var request = _handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            ValidateRequestParameter(fields, "client_id", "custom");
            ValidateRequestParameter(fields, "custom", "custom");
            ValidateRequestParameter(fields, "global", "global");
        }

        [Fact]
        public async Task Local_custom_parameters_should_not_interfere_with_global()
        {
            var globalOptions = new PushedAuthorizationClientOptions { Parameters = { { "global", "value" } } };
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, globalOptions);

            var localParameters = new Parameters
            {
                { "client_id", "custom" },
                { "custom", "custom" }
            };
            
            _ = await pushedAuthorizationClient.PushAuthorizationAsync(parameters: localParameters);

            globalOptions.Parameters.Should().HaveCount(1);
            var globalValue = globalOptions.Parameters.FirstOrDefault(p => p.Key == "global").Value;
            globalValue.Should().Be("value");
        }

        [Fact]
        public async Task Setting_basic_authentication_style_should_send_basic_authentication_header()
        {
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions
            {
                ClientId = "client",
                ClientSecret = "secret",
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(new Parameters());

            var request = _handler.Request;

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.Scheme.Should().Be("Basic");
            request.Headers.Authorization.Parameter.Should().Be(BasicAuthenticationOAuthHeaderValue.EncodeCredential("client", "secret"));
        }

        [Fact]
        public async Task Setting_post_values_authentication_style_should_post_values()
        {
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions
            {
                ClientId = "client",
                ClientSecret = "secret",
                ClientCredentialStyle = ClientCredentialStyle.PostBody
            });

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(new Parameters());

            var request = _handler.Request;
            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields["client_id"].First().Should().Be("client");
            fields["client_secret"].First().Should().Be("secret");

        }

        // REVIEW - I don't think this is actually a valid PAR, since the client didn't authenticate. 
        [Fact]
        public async Task Setting_client_id_only_and_post_should_put_client_id_in_post_body()
        {
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions
            {
                ClientId = "client",
                ClientCredentialStyle = ClientCredentialStyle.PostBody
            });

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(new Parameters());

            var request = _handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(_handler.Body);
            fields["client_id"].First().Should().Be("client");
        }

        // REVIEW - I don't think this is actually a valid PAR, since the client didn't authenticate. 
        [Fact]
        public async Task Setting_client_id_only_and_header_should_put_client_id_in_header()
        {
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions
            {
                ClientId = "client",
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            });

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(new Parameters());

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
            var pushedAuthorizationClient = new PushedAuthorizationClient(_client, new PushedAuthorizationClientOptions
            {
                ClientId = "client",
                ClientAssertion = { Type = "type", Value = "value" }
            });

            var response = await pushedAuthorizationClient.PushAuthorizationAsync(new Parameters());
            var fields = QueryHelpers.ParseQuery(_handler.Body);

            fields["client_id"].First().Should().Be("client");
            fields["client_assertion_type"].First().Should().Be("type");
            fields["client_assertion"].First().Should().Be("value");
        }


        void ValidateRequestParameter(Dictionary<string, StringValues> fields, string name, string expectedValue)
        {
            fields.TryGetValue(name, out var values).Should().BeTrue();
            values.First().Should().Be(expectedValue);
        }
    }
}
