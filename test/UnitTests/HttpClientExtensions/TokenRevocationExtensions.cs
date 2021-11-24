// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    public class TokenRevocationExtensionsTests
    {
        private const string Endpoint = "http://server/endoint";

        [Fact]
        public async Task Http_request_should_have_correct_format()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var request = new TokenRevocationRequest
            {
                Address = Endpoint, 
                Token = "token"
            };

            request.Headers.Add("custom", "custom");
            request.Properties.Add("custom", "custom");

            var response = await client.RevokeTokenAsync(request);

            var httpRequest = handler.Request;

            httpRequest.Method.Should().Be(HttpMethod.Post);
            httpRequest.RequestUri.Should().Be(new Uri(Endpoint));
            httpRequest.Content.Should().NotBeNull();

            var headers = httpRequest.Headers;
            headers.Count().Should().Be(2);
            headers.Should().Contain(h => h.Key == "custom" && h.Value.First() == "custom");

            var properties = httpRequest.Properties;
            properties.Count.Should().Be(1);

            var prop = properties.First();
            prop.Key.Should().Be("custom");
            ((string)prop.Value).Should().Be("custom");
        }

        [Fact]
        public async Task Valid_protocol_response_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.OK, "ok");
            var client = new HttpClient(handler);

            var response = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = Endpoint,
                Token = "token",
                ClientId = "client"
            });

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Repeating_a_request_should_succeed()
        {
            var handler = new NetworkHandler(HttpStatusCode.OK, "ok");
            var client = new HttpClient(handler);

            var request = new TokenRevocationRequest
            {
                Address = Endpoint,
                Token = "token",
                ClientId = "client"
            };

            var response = await client.RevokeTokenAsync(request);

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);

            // repeat
            response = await client.RevokeTokenAsync(request);

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Valid_protocol_error_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("failure_token_revocation_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.BadRequest);
            var client = new HttpClient(handler);

            var response = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = Endpoint,
                Token = "token",
                ClientId = "client"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Error.Should().Be("error");
        }

        [Fact]
        public async Task Malformed_response_document_should_be_handled_correctly()
        {
            var document = "invalid";
            var handler = new NetworkHandler(document, HttpStatusCode.BadRequest);
            var client = new HttpClient(handler);

            var response = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = Endpoint,
                Token = "token",
                ClientId = "client"
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

            var response = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = Endpoint,
                Token = "token",
                ClientId = "client"
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

            var response = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = Endpoint,
                Token = "token",
                ClientId = "client"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("not found");
        }

        [Fact]
        public async Task Additional_parameters_should_be_sent_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.OK, "ok");
            var client = new HttpClient(handler);

            var response = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = Endpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Token = "token",
                Parameters =
                {
                    { "foo", "bar" }
                }
            });

            // check request
            var fields = QueryHelpers.ParseQuery(handler.Body);
            fields.Count.Should().Be(2);
            
            fields["token"].First().Should().Be("token");
            fields["foo"].First().Should().Be("bar");

            // check response
            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}