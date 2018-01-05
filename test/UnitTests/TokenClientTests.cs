// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

using Xunit;
using Newtonsoft.Json;

namespace IdentityModel.UnitTests
{
    public class TokenClientTests
    {
        const string Endpoint = "http://server/token";
        
        [Fact]
        public async Task Valid_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new TokenClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.ExpiresIn.Should().Be(3600);
            response.AccessToken.Should().Be("access_token");
            response.RefreshToken.Should().Be("refresh_token");
            response.TryGet("custom").Should().Be("custom");
        }

        [Fact]
        public async Task Valid_protocol_error_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("failure_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.BadRequest);

            var client = new TokenClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Error.Should().Be("error");
            response.ErrorDescription.Should().Be("error_description");
            response.TryGet("custom").Should().Be("custom");
        }

        [Fact]
        public async Task Malformed_response_document_should_be_handled_correctly()
        {
            var document = "invalid";
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new TokenClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Raw.Should().Be("invalid");
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(new Exception("exception"));

            var client = new TokenClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Error.Should().Be("exception");
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new TokenClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("not found");
        }

        [Fact]
        public async Task Http_error_with_non_json_content_should_be_handled_correctly()
        {
            var handler = new NetworkHandler("not_json", HttpStatusCode.Unauthorized);

            var client = new TokenClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Error.Should().Be("Unauthorized");
            response.Raw.Should().Be("not_json");
        }

        [Fact]
        public async Task Http_error_with_json_content_should_be_handled_correctly()
        {
            var content = new
            {
                foo = "foo",
                bar = "bar"
            };

            var handler = new NetworkHandler(JsonConvert.SerializeObject(content), HttpStatusCode.Unauthorized);

            var client = new TokenClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Error.Should().Be("Unauthorized");

            response.Json.TryGetString("foo").Should().Be("foo");
            response.Json.TryGetString("bar").Should().Be("bar");
        }

        [Fact]
        public async Task Setting_basic_authentication_style_should_send_basic_authentication_header()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new TokenClient(
                Endpoint,
                "client",
                "secret",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();
            var request = handler.Request;

            request.Headers.Authorization.Should().NotBeNull();
            request.Headers.Authorization.Scheme.Should().Be("Basic");
            request.Headers.Authorization.Parameter.Should().Be(Convert.ToBase64String(Encoding.UTF8.GetBytes("client:secret")));
        }

        [Fact]
        public async Task Setting_post_values_authentication_style_should_post_values()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new TokenClient(
                Endpoint,
                "client",
                "secret",
                style: AuthenticationStyle.PostValues,
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();
            var request = handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(handler.Body);
            fields["client_id"].First().Should().Be("client");
            fields["client_secret"].First().Should().Be("secret");

        }

        [Fact]
        public async Task Setting_no_client_id_and_secret_should_not_send_credentials()
        {
             var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new TokenClient(
                Endpoint,
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();
            var request = handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(handler.Body);
            
            fields.TryGetValue("client_secret", out _).Should().BeFalse();
            fields.TryGetValue("client_id", out _).Should().BeFalse();
        }

        [Fact]
        public async Task Setting_client_id_only_should_put_client_id_in_post_body()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new TokenClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.RequestClientCredentialsAsync();
            var request = handler.Request;

            request.Headers.Authorization.Should().BeNull();

            var fields = QueryHelpers.ParseQuery(handler.Body);
            fields["client_id"].First().Should().Be("client");
        }
    }
}