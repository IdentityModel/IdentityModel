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
    public class HttpClientDiscoveryExtensionsTests
    {
        NetworkHandler _successHandler;
        string _endpoint = "https://demo.identityserver.io/.well-known/openid-configuration";
        string _authority = "https://demo.identityserver.io";

        public HttpClientDiscoveryExtensionsTests()
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

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Explicit_address_should_work()
        {
            var client = new HttpClient(_successHandler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _endpoint
            });

            disco.IsError.Should().BeFalse();
        }

        [Fact]
        public async Task Authority_should_expand_to_endpoint()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");
            var client = new HttpClient(handler);

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _authority
            });

            disco.IsError.Should().BeTrue();
            handler.Request.RequestUri.Should().Be(_endpoint);
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Http);
            disco.Error.Should().StartWith("Error connecting to");
            disco.Error.Should().EndWith("not found");
            disco.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Policy_authority_does_not_get_overwritten()
        {
            var policy = new DiscoveryPolicy
            {
                Authority = "https://server:123"
            };

            var client = new HttpClient(_successHandler);
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _endpoint,
                Policy = policy
            });

            disco.IsError.Should().BeTrue();
            policy.Authority.Should().Be("https://server:123");
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(new Exception("error"));

            var client = new HttpClient(handler);
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _endpoint,
            });

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Exception);
            disco.Error.Should().StartWith("Error connecting to");
            disco.Error.Should().EndWith("error");
        }

        [Fact]
        public async Task TryGetValue_calls_should_behave_as_excected()
        {
            var client = new HttpClient(_successHandler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeFalse();

            disco.TryGetValue(OidcConstants.Discovery.AuthorizationEndpoint).Should().NotBeNull();
            disco.TryGetValue("unknown").Should().BeNull();

            disco.TryGetString(OidcConstants.Discovery.AuthorizationEndpoint).Should().Be("https://demo.identityserver.io/connect/authorize");
            disco.TryGetString("unknown").Should().BeNull();
        }

        [Fact]
        public async Task Strongly_typed_accessors_should_behave_as_expected()
        {
            var client = new HttpClient(_successHandler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeFalse();

            disco.TokenEndpoint.Should().Be("https://demo.identityserver.io/connect/token");
            disco.AuthorizeEndpoint.Should().Be("https://demo.identityserver.io/connect/authorize");
            disco.UserInfoEndpoint.Should().Be("https://demo.identityserver.io/connect/userinfo");

            disco.FrontChannelLogoutSupported.Should().Be(true);
            disco.FrontChannelLogoutSessionSupported.Should().Be(true);

            var responseModes = disco.ResponseModesSupported;

            responseModes.Should().Contain("form_post");
            responseModes.Should().Contain("query");
            responseModes.Should().Contain("fragment");
        }

        [Fact]
        public async Task Http_error_with_non_json_content_should_be_handled_correctly()
        {
            var handler = new NetworkHandler("not_json", HttpStatusCode.InternalServerError);
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Http);
            disco.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            disco.Error.Should().Contain("Internal Server Error");
            disco.Raw.Should().Be("not_json");
            disco.Json.Should().BeNull();
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

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Http);
            disco.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            disco.Error.Should().Contain("Internal Server Error");

            disco.Json.TryGetString("foo").Should().Be("foo");
            disco.Json.TryGetString("bar").Should().Be("bar");
        }

        [Fact]
        public async Task Http_error_at_jwk_with_non_json_content_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(request =>
            {
                HttpResponseMessage response;

                if (!request.RequestUri.AbsoluteUri.Contains("jwk"))
                {
                    var discoFileName = FileName.Create("discovery.json");
                    var document = File.ReadAllText(discoFileName);

                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(document)
                    };
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("not_json")
                    };
                }

                return response;
            });

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Http);
            disco.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            disco.Error.Should().Contain("Internal Server Error");
            disco.Raw.Should().Be("not_json");
            disco.Json.Should().BeNull();
        }

        [Fact]
        public async Task Http_error_at_jwk_with_json_content_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(request =>
            {
                HttpResponseMessage response;

                if (!request.RequestUri.AbsoluteUri.Contains("jwk"))
                {
                    var discoFileName = FileName.Create("discovery.json");
                    var document = File.ReadAllText(discoFileName);

                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(document)
                    };
                }
                else
                {
                    var content = new
                    {
                        foo = "foo",
                        bar = "bar"
                    };

                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(content))
                    }; 
                }

                return response;
            });

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Http);
            disco.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            disco.Error.Should().Contain("Internal Server Error");

            disco.Json.TryGetString("foo").Should().Be("foo");
            disco.Json.TryGetString("bar").Should().Be("bar");
        }
    }
}