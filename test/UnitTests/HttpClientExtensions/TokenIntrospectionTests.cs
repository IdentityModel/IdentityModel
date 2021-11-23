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

            var response = await client.IntrospectTokenAsync(request);

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
            response.Claims.Should().NotBeEmpty();

            var audiences = response.Claims.Where(c => c.Type == "aud").ToList();
            audiences.Count().Should().Be(2);
            audiences.First().Value.Should().Be("https://idsvr4/resources");
            audiences.Skip(1).First().Value.Should().Be("api1");

            response.Claims.First(c => c.Type == "iss").Value.Should().Be("https://idsvr4");
            response.Claims.First(c => c.Type == "nbf").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "exp").Value.Should().Be("1475828471");
            response.Claims.First(c => c.Type == "client_id").Value.Should().Be("client");
            response.Claims.First(c => c.Type == "sub").Value.Should().Be("1");
            response.Claims.First(c => c.Type == "auth_time").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "idp").Value.Should().Be("local");
            response.Claims.First(c => c.Type == "amr").Value.Should().Be("password");
            response.Claims.First(c => c.Type == "active").Value.Should().Be("true");

            var scopes = response.Claims.Where(c => c.Type == "scope").ToList();
            scopes.Count().Should().Be(2);
            scopes.First().Value.Should().Be("api1");
            scopes.First().Issuer.Should().Be("https://idsvr4");
            scopes.Skip(1).First().Value.Should().Be("api2");
            scopes.Skip(1).First().Issuer.Should().Be("https://idsvr4");
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
            response.Claims.Should().NotBeEmpty();

            var audiences = response.Claims.Where(c => c.Type == "aud").ToList();
            audiences.Count().Should().Be(2);
            audiences.First().Value.Should().Be("https://idsvr4/resources");
            audiences.Skip(1).First().Value.Should().Be("api1");

            response.Claims.First(c => c.Type == "nbf").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "exp").Value.Should().Be("1475828471");
            response.Claims.First(c => c.Type == "client_id").Value.Should().Be("client");
            response.Claims.First(c => c.Type == "sub").Value.Should().Be("1");
            response.Claims.First(c => c.Type == "auth_time").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "idp").Value.Should().Be("local");
            response.Claims.First(c => c.Type == "amr").Value.Should().Be("password");
            response.Claims.First(c => c.Type == "active").Value.Should().Be("true");

            var scopes = response.Claims.Where(c => c.Type == "scope").ToList();
            scopes.Count().Should().Be(2);
            scopes.First().Value.Should().Be("api1");
            scopes.Skip(1).First().Value.Should().Be("api2");
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
            response.Claims.Should().NotBeEmpty();

            var audiences = response.Claims.Where(c => c.Type == "aud").ToList();
            audiences.Count().Should().Be(2);
            audiences.First().Value.Should().Be("https://idsvr4/resources");
            audiences.Skip(1).First().Value.Should().Be("api1");

            response.Claims.First(c => c.Type == "iss").Value.Should().Be("https://idsvr4");
            response.Claims.First(c => c.Type == "nbf").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "exp").Value.Should().Be("1475828471");
            response.Claims.First(c => c.Type == "client_id").Value.Should().Be("client");
            response.Claims.First(c => c.Type == "sub").Value.Should().Be("1");
            response.Claims.First(c => c.Type == "auth_time").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "idp").Value.Should().Be("local");
            response.Claims.First(c => c.Type == "amr").Value.Should().Be("password");
            response.Claims.First(c => c.Type == "active").Value.Should().Be("true");

            var scopes = response.Claims.Where(c => c.Type == "scope").ToList();
            scopes.Count().Should().Be(2);
            scopes.First().Value.Should().Be("api1");
            scopes.First().Issuer.Should().Be("https://idsvr4");
            scopes.Skip(1).First().Value.Should().Be("api2");
            scopes.Skip(1).First().Issuer.Should().Be("https://idsvr4");

            // repeat
            response = await client.IntrospectTokenAsync(request);

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().NotBeEmpty();

            audiences = response.Claims.Where(c => c.Type == "aud").ToList();
            audiences.Count().Should().Be(2);
            audiences.First().Value.Should().Be("https://idsvr4/resources");
            audiences.Skip(1).First().Value.Should().Be("api1");

            response.Claims.First(c => c.Type == "iss").Value.Should().Be("https://idsvr4");
            response.Claims.First(c => c.Type == "nbf").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "exp").Value.Should().Be("1475828471");
            response.Claims.First(c => c.Type == "client_id").Value.Should().Be("client");
            response.Claims.First(c => c.Type == "sub").Value.Should().Be("1");
            response.Claims.First(c => c.Type == "auth_time").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "idp").Value.Should().Be("local");
            response.Claims.First(c => c.Type == "amr").Value.Should().Be("password");
            response.Claims.First(c => c.Type == "active").Value.Should().Be("true");

            scopes = response.Claims.Where(c => c.Type == "scope").ToList();
            scopes.Count().Should().Be(2);
            scopes.First().Value.Should().Be("api1");
            scopes.First().Issuer.Should().Be("https://idsvr4");
            scopes.Skip(1).First().Value.Should().Be("api2");
            scopes.Skip(1).First().Issuer.Should().Be("https://idsvr4");
        }

        [Fact]
        public void Request_without_token_should_fail()
        {
            var document = File.ReadAllText(FileName.Create("success_introspection_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(Endpoint)
            };

            Func<Task> act = async () => await client.IntrospectTokenAsync(new TokenIntrospectionRequest());

            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("token");
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
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(new Exception("exception"));

            var client = new HttpClient(handler);
            var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
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
            response.Claims.Should().NotBeEmpty();

            var audiences = response.Claims.Where(c => c.Type == "aud").ToList();
            audiences.Count().Should().Be(2);
            audiences.First().Value.Should().Be("https://idsvr4/resources");
            audiences.Skip(1).First().Value.Should().Be("api1");

            response.Claims.First(c => c.Type == "iss").Value.Should().Be("https://idsvr4");
            response.Claims.First(c => c.Type == "nbf").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "exp").Value.Should().Be("1475828471");
            response.Claims.First(c => c.Type == "client_id").Value.Should().Be("client");
            response.Claims.First(c => c.Type == "sub").Value.Should().Be("1");
            response.Claims.First(c => c.Type == "auth_time").Value.Should().Be("1475824871");
            response.Claims.First(c => c.Type == "idp").Value.Should().Be("local");
            response.Claims.First(c => c.Type == "amr").Value.Should().Be("password");
            response.Claims.First(c => c.Type == "active").Value.Should().Be("true");

            var scopes = response.Claims.Where(c => c.Type == "scope").ToList();
            scopes.Count().Should().Be(2);
            scopes.First().Value.Should().Be("api1");
            scopes.First().Issuer.Should().Be("https://idsvr4");
            scopes.Skip(1).First().Value.Should().Be("api2");
            scopes.Skip(1).First().Issuer.Should().Be("https://idsvr4");
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
                    { "scope", "scope1 scope2" },
                    { "foo", "bar" }
                }
            });

            // check request
            var fields = QueryHelpers.ParseQuery(handler.Body);
            fields.Count.Should().Be(3);
            
            fields["token"].First().Should().Be("token");
            fields["scope"].First().Should().Be("scope1 scope2");
            fields["foo"].First().Should().Be("bar");

            // check response
            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().NotBeEmpty();
        }
    }
}