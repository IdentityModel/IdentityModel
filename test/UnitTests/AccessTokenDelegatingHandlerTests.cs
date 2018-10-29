using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class AccessTokenDelegatingHandlerTests
    {
        [Fact]
        public async Task The_401_response_that_causes_token_refresh_and_retry_should_be_disposed_to_unblock_socket()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            using (var tokenClient = new TokenClient(
                "http://server/token",
                "client",
                handler))
            {
                var indirectOutputOfHttpResponses = new StubHttpResponsesHandler();
                var accessTokenHandler = new AccessTokenDelegatingHandler(
                    tokenClient,
                    "scope",
                    innerHandler: indirectOutputOfHttpResponses);

                var apiClient = new HttpClient(accessTokenHandler);

                await apiClient.GetStringAsync("http://someapi/somecall");

                indirectOutputOfHttpResponses.FirstAttempt401Response
                    .Disposed
                    .Should()
                    .BeTrue("Unauthorized response should be disposed to avoid socket blocking");
            }
        }

        [Fact]
        public void Creating_with_token_client_should_not_throw()
        {
            var tokenClient = new TokenClient("http://server/token", "client");
            Action act = () => new AccessTokenDelegatingHandler(tokenClient, "scope");

            act.Should().NotThrow();
        }

        [Fact]
        public void Creating_should_not_throw()
        {
            Action act = () => new AccessTokenDelegatingHandler("http://server/token", "client", "secret", "scope");

            act.Should().NotThrow();
        }

        [Fact]
        public async Task Simultaneously_100_requests_should_all_be_successful()
        {
            var document = File.ReadAllText(FileName.Create("success_token_response.json"));
            var tokenClientHandler = new TestTokenClientHttpMessageHandler(document);
            var tokenClient = new TokenClient("http://server/token", "client", tokenClientHandler);

            var httpClientHandler = new TestHttpClientMessageHandler();
            var accessTokenHandler = new AccessTokenDelegatingHandler(
                tokenClient,
                "scope",
                innerHandler: httpClientHandler);
            var apiClient = new HttpClient(accessTokenHandler);

            var successRequests = 0;
            var tasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var result = await apiClient.PostAsync("http://someapi/somecall", new StringContent("some data"));
                    if (result.IsSuccessStatusCode)
                    {
                        Interlocked.Increment(ref successRequests);
                    }
                }));
            }

            await Task.WhenAll(tasks);

            httpClientHandler.RequestCounter.Should().Be(tasks.Count);
            httpClientHandler.RequestsWithoutTokenValueCounter.Should().Be(0);
            successRequests.Should().Be(tasks.Count);
            tokenClientHandler.RequestCounter.Should().BeLessOrEqualTo(2);
        }

        private class TestTokenClientHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _tokenResponseString;

            private int _requestCounter;

            public TestTokenClientHttpMessageHandler(string tokenResponseString)
            {
                _tokenResponseString = tokenResponseString;
            }

            public int RequestCounter => _requestCounter;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _requestCounter);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(_tokenResponseString);
                return Task.FromResult(response);
            }
        }

        private class TestHttpClientMessageHandler : HttpMessageHandler
        {
            private int _requestCounter;
            private int _requestsWithoutTokenValueCounter;

            public int RequestCounter => _requestCounter;
            public int RequestsWithoutTokenValueCounter => _requestsWithoutTokenValueCounter;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _requestCounter);

                if (string.IsNullOrEmpty(request.Headers.Authorization.Parameter))
                {
                    Interlocked.Increment(ref _requestsWithoutTokenValueCounter);
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }
}