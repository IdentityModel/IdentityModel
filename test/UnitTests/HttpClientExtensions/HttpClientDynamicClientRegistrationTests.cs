// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class HttpClientDynamicClientRegistrationTests
    {
        const string Endpoint = "http://server/register";

        [Fact]
        public async Task Valid_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_registration_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.Created);

            var client = new HttpClient(handler);
            var response = await client.RegisterClientAsync(new DynamicClientRegistrationRequest
            {
                Address = Endpoint,
                RegistrationRequest = new RegistrationRequest()
            });

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);

            response.ClientId.Should().Be("s6BhdRkqt3");
            response.ClientSecret.Should().Be("ZJYCqe3GGRvdrudKyZS0XhGv_Z45DuKhCUk0gBR1vZk");
            response.ClientSecretExpiresAt.Should().Be(1577858400);
            response.ClientIdIssuedAt.Should().NotHaveValue();
            response.RegistrationAccessToken.Should().Be("this.is.an.access.token.value.ffx83");
            response.RegistrationClientUri.Should()
                .Be("https://server.example.com/connect/register?client_id=s6BhdRkqt3");

            response.Json.TryGetString(OidcConstants.ClientMetadata.TokenEndpointAuthenticationMethod)
                .Should()
                .Be(OidcConstants.EndpointAuthenticationMethods.BasicAuthentication);
        }

        [Fact]
        public async Task Malformed_response_document_should_be_handled_correctly()
        {
            var document = "invalid";
            var handler = new NetworkHandler(document, HttpStatusCode.Created);

            var client = new HttpClient(handler);
            var response = await client.RegisterClientAsync(new DynamicClientRegistrationRequest
            {
                Address = Endpoint,
                RegistrationRequest = new RegistrationRequest()
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Raw.Should().Be("invalid");
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(new Exception("exception"));

            var client = new HttpClient(handler);
            var response = await client.RegisterClientAsync(new DynamicClientRegistrationRequest
            {
                Address = Endpoint,
                RegistrationRequest = new RegistrationRequest()
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Error.Should().Be("exception");
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var response = await client.RegisterClientAsync(new DynamicClientRegistrationRequest
            {
                Address = Endpoint,
                RegistrationRequest = new RegistrationRequest()
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("not found");
        }

        [Fact]
        public async Task Valid_protocol_error_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("failure_registration_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.BadRequest);

            var client = new HttpClient(handler);
            var response = await client.RegisterClientAsync(new DynamicClientRegistrationRequest
            {
                Address = Endpoint,
                RegistrationRequest = new RegistrationRequest()
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Error.Should().Be("invalid_redirect_uri");
            response.ErrorDescription.Should().Be("One or more redirect_uri values are invalid");
            response.TryGet("custom").Should().Be("custom");
        }
    }
}