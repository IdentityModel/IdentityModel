// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class DiscoveryExtensionsTests
    {
        private readonly NetworkHandler _successHandler;
        private readonly string _endpoint = "https://demo.identityserver.io/.well-known/openid-configuration";
        private readonly string _authority = "https://demo.identityserver.io";

        public DiscoveryExtensionsTests()
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
        public async Task Http_request_should_have_correct_format()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var request = new DiscoveryDocumentRequest
            {
                Address = _endpoint
            };

            request.Headers.Add("custom", "custom");
            request.Properties.Add("custom", "custom");

            var response = await client.GetDiscoveryDocumentAsync(request);

            var httpRequest = handler.Request;

            httpRequest.Method.Should().Be(HttpMethod.Get);
            httpRequest.RequestUri.Should().Be(new Uri(_endpoint));
            httpRequest.Content.Should().BeNull();

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
            disco.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
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
                Address = _endpoint
            });

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Exception);
            disco.Error.Should().StartWith("Error connecting to");
            disco.Error.Should().EndWith("error.");
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
            disco.TryGetValue("unknown").ValueKind.Should().Be(JsonValueKind.Undefined);

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

            var responseModes = disco.ResponseModesSupported.ToList();

            responseModes.Should().Contain("form_post");
            responseModes.Should().Contain("query");
            responseModes.Should().Contain("fragment");

            disco.KeySet.Keys.Count.Should().Be(1);
            disco.KeySet.Keys.First().Kid.Should().Be("a3rMUgMFv9tPclLa6yF3zAkfquE");
            
            disco.MtlsEndpointAliases.Should().NotBeNull();
            disco.MtlsEndpointAliases.TokenEndpoint.Should().BeNull();
        }
        
        [Fact]
        public async Task Mtls_alias_accessors_should_behave_as_expected()
        {
            var discoFileName = FileName.Create("discovery_mtls.json");
            var document = File.ReadAllText(discoFileName);

            var jwksFileName = FileName.Create("discovery_jwks.json");
            var jwks = File.ReadAllText(jwksFileName);

            var handler = new NetworkHandler(request =>
            {
                if (request.RequestUri.AbsoluteUri.EndsWith("jwks"))
                {
                    return jwks;
                }

                return document;
            }, HttpStatusCode.OK);
            
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeFalse();
            disco.MtlsEndpointAliases.Should().NotBeNull();

            disco.MtlsEndpointAliases.TokenEndpoint.Should().Be("https://mtls.identityserver.io/connect/token");
            disco.MtlsEndpointAliases.Json.TryGetString(OidcConstants.Discovery.TokenEndpoint).Should().Be("https://mtls.identityserver.io/connect/token");
            
            disco.MtlsEndpointAliases.RevocationEndpoint.Should().Be("https://mtls.identityserver.io/connect/revocation");
            disco.MtlsEndpointAliases.IntrospectionEndpoint.Should().Be("https://mtls.identityserver.io/connect/introspect");
            disco.MtlsEndpointAliases.DeviceAuthorizationEndpoint.Should().Be("https://mtls.identityserver.io/connect/deviceauthorization");
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
            disco.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            disco.Error.Should().Contain("Internal Server Error");
            disco.Raw.Should().Be("not_json");
            disco.Json.ValueKind.Should().Be(JsonValueKind.Undefined);
        }

        [Fact]
        public async Task Http_error_with_json_content_should_be_handled_correctly()
        {
            var content = new
            {
                foo = "foo",
                bar = "bar"
            };

            var handler = new NetworkHandler(JsonSerializer.Serialize(content), HttpStatusCode.InternalServerError);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_endpoint)
            };

            var disco = await client.GetDiscoveryDocumentAsync();

            disco.IsError.Should().BeTrue();
            disco.ErrorType.Should().Be(ResponseErrorType.Http);
            disco.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
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
            disco.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            disco.Error.Should().Contain("Internal Server Error");
            disco.Raw.Should().Be("not_json");
            disco.Json.ValueKind.Should().Be(JsonValueKind.Undefined);
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
                        Content = new StringContent(JsonSerializer.Serialize(content))
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
            disco.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            disco.Error.Should().Contain("Internal Server Error");

            disco.Json.TryGetString("foo").Should().Be("foo");
            disco.Json.TryGetString("bar").Should().Be("bar");
        }
    }
}