// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class HttpClientJsonWebkeyExtensionsTests
    {
        readonly NetworkHandler _successHandler;
        readonly string _endpoint = "https://demo.identityserver.io/.well-known/openid-configuration/jwks";
        
        public HttpClientJsonWebkeyExtensionsTests()
        {
            var discoFileName = FileName.Create("discovery.json");
            var document = File.ReadAllText(discoFileName);

            var jwksFileName = FileName.Create("discovery_jwks.json");
            var jwks = File.ReadAllText(jwksFileName);

            _successHandler = new NetworkHandler(request =>
            {
                if (request.RequestUri.AbsoluteUri.EndsWith("jwks"))
                {
                    return jwks;
                }

                return document;
            }, HttpStatusCode.OK);
        }

        [Fact]
        public async Task Base_address_should_work()
        {
            var client = new HttpClient(_successHandler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var jwk = await client.GetJsonWebKeySetAsync();

            jwk.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Explicit_address_should_work()
        {
            var client = new HttpClient(_successHandler);

            var jwk = await client.GetJsonWebKeySetAsync(_endpoint);

            jwk.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var jwk = await client.GetJsonWebKeySetAsync();

            jwk.IsError.Should().BeTrue();
            jwk.ErrorType.Should().Be(ResponseErrorType.Http);
            jwk.Error.Should().StartWith("Error connecting to");
            jwk.Error.Should().EndWith("not found");
            jwk.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(new Exception("error"));

            var client = new HttpClient(handler);
            var jwk = await client.GetJsonWebKeySetAsync(_endpoint);

            jwk.IsError.Should().BeTrue();
            jwk.ErrorType.Should().Be(ResponseErrorType.Exception);
            jwk.Error.Should().StartWith("Error connecting to");
            jwk.Error.Should().EndWith("error.");
        }

        [Fact]
        public async Task Strongly_typed_accessors_should_behave_as_expected()
        {
            var client = new HttpClient(_successHandler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var jwk = await client.GetJsonWebKeySetAsync();

            jwk.IsError.Should().BeFalse();
            jwk.KeySet.Should().NotBeNull();
        }

        [Fact]
        public async Task Http_error_with_non_json_content_should_be_handled_correctly()
        {
            var handler = new NetworkHandler("not_json", HttpStatusCode.InternalServerError);
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var jwk = await client.GetJsonWebKeySetAsync();

            jwk.IsError.Should().BeTrue();
            jwk.ErrorType.Should().Be(ResponseErrorType.Http);
            jwk.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            jwk.Error.Should().Contain("Internal Server Error");
            jwk.Raw.Should().Be("not_json");
            jwk.Json.Should().BeNull();
        }

        [Fact]
        public async Task Http_error_with_json_content_should_be_handled_correctly()
        {
            var content = new
            {
                foo = "foo",
                bar = "bar"
            };

            var handler = new NetworkHandler(JsonConvert.SerializeObject(content), HttpStatusCode.InternalServerError);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var jwk = await client.GetJsonWebKeySetAsync();

            jwk.IsError.Should().BeTrue();
            jwk.ErrorType.Should().Be(ResponseErrorType.Http);
            jwk.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            jwk.Error.Should().Contain("Internal Server Error");

            jwk.Json.TryGetString("foo").Should().Be("foo");
            jwk.Json.TryGetString("bar").Should().Be("bar");
        }
    }
}