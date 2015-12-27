using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace IdentityModel.Tests
{
    public class Introspection
    {
        IntrospectionRequest request = new IntrospectionRequest
        {
            Token = "token"
        };

        [Fact]
        public async Task Unauthorized()
        {
            var handler = new IntrospectionHttpHandler();
            var client = CreateClient(handler);

            var response = await client.SendAsync(request);

            response.IsError.Should().Be(true);
            response.Error.Should().Be("Unauthorized");
        }

        [Fact]
        public async Task InActive()
        {
            var payload = new Dictionary<string, object>
            {
                { "active", false }
            };

            var handler = new IntrospectionHttpHandler(payload);
            var client = CreateClient(handler);

            var response = await client.SendAsync(request);

            response.IsError.Should().Be(false);
            response.IsActive.Should().Be(false);
        }

        [Fact]
        public async Task Active()
        {
            var payload = new Dictionary<string, object>
            {
                { "active", true }
            };

            var handler = new IntrospectionHttpHandler(payload);
            var client = CreateClient(handler);

            var response = await client.SendAsync(request);

            response.IsError.Should().Be(false);
            response.IsActive.Should().Be(true);
        }

        [Fact]
        public async Task ActiveSimpleClaims()
        {
            var payload = new Dictionary<string, object>
            {
                { "active", true },
                { "claim1", "value1" },
                { "claim2", "value2" },
                { "claim3", "value3" },
            };

            var handler = new IntrospectionHttpHandler(payload);
            var client = CreateClient(handler);

            var response = await client.SendAsync(request);

            response.IsError.Should().Be(false);
            response.IsActive.Should().Be(true);
            response.Claims.Count().Should().Be(4);
        }

        [Fact]
        public async Task ActiveComplexClaims()
        {
            var payload = new Dictionary<string, object>
            {
                { "active", true },
                { "claim1", new[] { "value1", "value2", "value3" } },
                { "claim2", "value2" },
                { "claim3", "value3" },
            };

            var handler = new IntrospectionHttpHandler(payload);
            var client = CreateClient(handler);

            var response = await client.SendAsync(request);

            response.IsError.Should().Be(false);
            response.IsActive.Should().Be(true);
            response.Claims.Count().Should().Be(6);
            response.Claims.Where(c => c.Item1 == "claim1").Count().Should().Be(3);
        }

        private IntrospectionClient CreateClient(IntrospectionHttpHandler handler)
        {
            return new IntrospectionClient("http://endpoint", innerHttpMessageHandler: handler);
        }
    }
}