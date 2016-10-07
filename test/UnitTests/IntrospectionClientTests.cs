using FluentAssertions;
using IdentityModel.Client;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class IntrospectionClientTests
    {
        const string Endpoint = "http://server/token";

        [Fact]
        public async Task Malformed_response_document_should_be_handled_correctly()
        {
            var document = "invalid";
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new IntrospectionClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Raw.Should().Be("invalid");
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(new Exception("exception"));

            var client = new IntrospectionClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Error.Should().Be("exception");
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new IntrospectionClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("not found");
        }

        [Fact]
        public async Task Valid_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "documents", "success_introspection_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new IntrospectionClient(
                Endpoint,
                "client",
                innerHttpMessageHandler: handler);

            var response = await client.SendAsync(new IntrospectionRequest { Token = "token" });

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            response.IsActive.Should().BeTrue();
            response.Claims.Should().NotBeEmpty();
        }
    }
}