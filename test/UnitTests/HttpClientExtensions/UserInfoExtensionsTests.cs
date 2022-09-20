// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class UserInfoExtensionsTests
    {
        private const string Endpoint = "http://server/endpoint";

        [Fact]
        public async Task Valid_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_userinfo_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = Endpoint,
                Token = "token"
            });

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.Claims.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Http_request_should_have_correct_format()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var request = new UserInfoRequest
            {
                Address = Endpoint,
                Token = "token"
            };

            request.Headers.Add("custom", "custom");
            request.Properties.Add("custom", "custom");

            var response = await client.GetUserInfoAsync(request);

            var httpRequest = handler.Request;

            httpRequest.Method.Should().Be(HttpMethod.Get);
            httpRequest.RequestUri.Should().Be(new Uri(Endpoint));
            httpRequest.Content.Should().BeNull();

            var headers = httpRequest.Headers;
            headers.Count().Should().Be(3);
            headers.Should().Contain(h => h.Key == "custom" && h.Value.First() == "custom");
            headers.Should().Contain(h => h.Key == "Authorization" && h.Value.First() == "Bearer token");

            var properties = httpRequest.Properties;
            properties.Count.Should().Be(1);

            var prop = properties.First();
            prop.Key.Should().Be("custom");
            ((string)prop.Value).Should().Be("custom");
        }


        [Fact]
        public async Task Malformed_response_document_should_be_handled_correctly()
        {
            var document = "invalid";
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = Endpoint,
                Token = "token"
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
            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = Endpoint,
                Token = "token"
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
            var response = await client.GetUserInfoAsync(new UserInfoRequest
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
        public async Task BadRequest_with_empty_body_should_be_handled_as_error()
        {
            var document = "";
            var handler = new NetworkHandler(document, HttpStatusCode.BadRequest);

            var client = new HttpClient(handler);
            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = Endpoint,
                Token = "token"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.Raw.Should().Be("");
            response.Error.Should().BeNull();
            response.Exception.Should().BeNull();
        }
    }
}