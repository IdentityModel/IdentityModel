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
    public class PushedAuthorizationTests
    {
        private const string Endpoint = "http://server/par";

        private PushedAuthorizationRequest Request = new PushedAuthorizationRequest
        {
            ClientId = "client",
            ResponseType = "code",
            Address = Endpoint
        };

        [Fact]
        public async Task Http_request_should_have_correct_format()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var request = new PushedAuthorizationRequest
            {
                ClientId = "client",
                ResponseType = "code",
                Address = Endpoint,
                RedirectUri = "https://example.com/signin-oidc",
                Scope = "openid profile",
                Nonce = "1234",
                State = "5678"
            };
            request.Headers.Add("custom", "custom");
            request.Properties.Add("custom", "custom");

            var response = await client.PushAuthorizationAsync(request);

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
        public async Task Request_with_request_object_should_succeed()
        {
            var document = File.ReadAllText(FileName.Create("success_par_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var request = new PushedAuthorizationRequest
            {
                ClientId = "client",
                Request = "request object value",
                Address = Endpoint,
            };
            
            await client.PushAuthorizationAsync(request);


            var fields = QueryHelpers.ParseQuery(handler.Body);
            fields.Count.Should().Be(2);

            fields["client_id"].First().Should().Be("client");
            fields["request"].First().Should().Be("request object value");
        }

        [Fact]
        public async Task Success_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_par_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(Endpoint)
            };

            var response = await client.PushAuthorizationAsync(Request);

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.RequestUri.Should().Be("urn:ietf:params:oauth:request_uri:123456");
            response.ExpiresIn.Should().Be(600);
        }

        [Fact]
        public async Task Malformed_response_document_should_be_handled_correctly()
        {
            var document = "invalid";
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.PushAuthorizationAsync(Request);

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
            var response = await client.PushAuthorizationAsync(Request);

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
            var response = await client.PushAuthorizationAsync(Request);

            response.IsError.Should().BeTrue();
            response.Error.Should().Be("not found");
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Additional_request_parameters_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_par_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.PushAuthorizationAsync(new PushedAuthorizationRequest
            {
                ClientId = "client",
                ResponseType = "code",
                Address = Endpoint,
                AcrValues = "idp:example",
                Scope = "scope1 scope2",
                Parameters =
                {
                    { "foo", "bar" }
                }
            });

            // check request
            var fields = QueryHelpers.ParseQuery(handler.Body);
            fields.Count.Should().Be(5);

            fields["client_id"].First().Should().Be("client");
            fields["response_type"].First().Should().Be("code");
            fields["acr_values"].First().Should().Be("idp:example");
            fields["scope"].First().Should().Be("scope1 scope2");
            fields["foo"].First().Should().Be("bar");

            // check response
            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Pushed_authorization_without_response_type_should_fail()
        {
            var document = File.ReadAllText(FileName.Create("success_par_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);
            var client = new HttpClient(handler);

            Request.ResponseType = null;

            Func<Task> act = async () => await client.PushAuthorizationAsync(Request);

            (await act.Should().ThrowAsync<ArgumentException>()).WithParameterName("response_type");
        }

        [Fact]
        public async Task Pushed_authorization_with_request_uri_should_fail()
        {
            var document = File.ReadAllText(FileName.Create("success_par_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);
            var client = new HttpClient(handler);

            Request.Parameters.Add(OidcConstants.AuthorizeRequest.RequestUri, "not allowed");


            Func<Task> act = async () => await client.PushAuthorizationAsync(Request);

            (await act.Should().ThrowAsync<ArgumentException>()).WithParameterName("request_uri");
        }
    }
}