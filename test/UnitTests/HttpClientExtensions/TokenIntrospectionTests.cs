// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class TokenIntrospectionTests
    {
        private const string Endpoint = "http://server/token";

        [Fact]
        public async Task Http_request_should_have_correct_format()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var request = new TokenIntrospectionRequest
            {
                Address = Endpoint,
                Token = "token"
            };

            request.Headers.Add("custom", "custom");
            request.Properties.Add("custom", "custom");

            _ = await client.IntrospectTokenAsync(request);

            var httpRequest = handler.Request;

            httpRequest.Method.Should().Be(HttpMethod.Post);
            httpRequest.RequestUri.Should().Be(new Uri(Endpoint));
            httpRequest.Content.Should().NotBeNull();
            httpRequest.Headers.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                ["Accept"] = new[] { "application/json" },
                ["custom"] = new[] { "custom" },
            });
            httpRequest.Properties.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                ["custom"] = "custom",
            });
        }

        [Fact]
        public async Task Success_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_introspection_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(Endpoint)
            };

            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Token = "token"
            });

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().BeEquivalentTo(new[]
            {
                new Claim("aud", "https://idsvr4/resources", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("aud", "api1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("iss", "https://idsvr4", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("nbf", "1475824871", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("exp", "1475828471", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("client_id", "client", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("sub", "1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("auth_time", "1475824871", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("idp", "local", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("amr", "password", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("active", "true", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("scope", "api1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("scope", "api2", ClaimValueTypes.String, "https://idsvr4"),
            });
        }

        [Fact]
        public async Task Success_protocol_response_without_issuer_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_introspection_response_no_issuer.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(Endpoint)
            };

            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Token = "token"
            });

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().BeEquivalentTo(new[]
            {
                new Claim("aud", "https://idsvr4/resources", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("aud", "api1", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("nbf", "1475824871", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("exp", "1475828471", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("client_id", "client", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("sub", "1", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("auth_time", "1475824871", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("idp", "local", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("amr", "password", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("active", "true", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("scope", "api1", ClaimValueTypes.String, "LOCAL AUTHORITY"),
                new Claim("scope", "api2", ClaimValueTypes.String, "LOCAL AUTHORITY"),
            });
        }

        [Fact]
        public async Task Repeating_a_request_should_succeed()
        {
            var document = File.ReadAllText(FileName.Create("success_introspection_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(Endpoint)
            };

            var request = new TokenIntrospectionRequest
            {
                Token = "token"
            };

            var response = await client.IntrospectTokenAsync(request);

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().BeEquivalentTo(new[]
            {
                new Claim("aud", "https://idsvr4/resources", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("aud", "api1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("iss", "https://idsvr4", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("nbf", "1475824871", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("exp", "1475828471", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("client_id", "client", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("sub", "1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("auth_time", "1475824871", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("idp", "local", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("amr", "password", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("active", "true", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("scope", "api1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("scope", "api2", ClaimValueTypes.String, "https://idsvr4"),
            });

            // repeat
            response = await client.IntrospectTokenAsync(request);

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().BeEquivalentTo(new[]
            {
                new Claim("aud", "https://idsvr4/resources", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("aud", "api1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("iss", "https://idsvr4", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("nbf", "1475824871", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("exp", "1475828471", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("client_id", "client", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("sub", "1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("auth_time", "1475824871", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("idp", "local", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("amr", "password", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("active", "true", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("scope", "api1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("scope", "api2", ClaimValueTypes.String, "https://idsvr4"),
            });
        }

        [Fact]
        public async Task Request_without_token_should_fail()
        {
            var document = File.ReadAllText(FileName.Create("success_introspection_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(Endpoint)
            };

            Func<Task> act = async () => await client.IntrospectTokenAsync(new TokenIntrospectionRequest());

            (await act.Should().ThrowAsync<ArgumentException>()).WithParameterName("token");
        }

        [Fact]
        public async Task Malformed_response_document_should_be_handled_correctly()
        {
            var document = "invalid";
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = Endpoint,
                Token = "token"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Raw.Should().Be("invalid");
            response.Exception.Should().BeAssignableTo<JsonException>();
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var exception = new Exception("exception");
            var handler = new NetworkHandler(exception);

            var client = new HttpClient(handler);
            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = Endpoint,
                Token = "token"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Error.Should().Be("exception");
            response.Exception.Should().BeSameAs(exception);
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = Endpoint,
                Token = "token"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("not found");
        }

        [Fact]
        public async Task Legacy_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("legacy_success_introspection_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = Endpoint,
                Token = "token"
            });

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().BeEquivalentTo(new[]
            {
                new Claim("aud", "https://idsvr4/resources", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("aud", "api1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("iss", "https://idsvr4", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("nbf", "1475824871", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("exp", "1475828471", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("client_id", "client", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("sub", "1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("auth_time", "1475824871", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("idp", "local", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("amr", "password", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("active", "true", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("scope", "api1", ClaimValueTypes.String, "https://idsvr4"),
                new Claim("scope", "api2", ClaimValueTypes.String, "https://idsvr4"),
            });
        }

        [Fact]
        public async Task Additional_request_parameters_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_introspection_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = Endpoint,
                ClientId = "client",
                Token = "token",
                Parameters =
                {
                    { "scope", "scope1" },
                    { "scope", "scope2" },
                    { "foo", "bar baz" }
                }
            });

            // check request
            handler.Body.Should().Be("scope=scope1&scope=scope2&foo=bar+baz&token=token");

            // check response
            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().NotBeEmpty();
        }
    }
}