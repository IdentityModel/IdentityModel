// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class AuthenticateDelegatingHandlerTests
    {
        [Fact]
        public async Task The_401_response_that_causes_token_refresh_and_retry_should_be_disposed_to_unblock_socket()
        {
            // arrange
            var authenticationCache = new InProcAuthenticationCache();
            var indirectOutputOfHttpResponses = new StubHttpResponsesHandler();
            var authenticateDelegatingHandler = new AuthenticateDelegatingHandler(authenticationCache, indirectOutputOfHttpResponses);

            var apiClient = new HttpClient(authenticateDelegatingHandler);

            // act
            await apiClient.GetStringAsync("http://someapi/somecall");

            // assert
            indirectOutputOfHttpResponses.FirstAttempt401Response
                .Disposed
                .Should()
                .BeTrue("Unauthorized response should be disposed to avoid socket blocking");
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(10, 25)]
        public async Task Decorating_requests_with_tokens_should_work_in_parallel(int interactions, int requests)
        {
            // arrange
            using (var appServer = new InProcHttpServer())
            using (var ssoServer = new InProcHttpServer())
            using (var tokenClient = new TokenClient("https://demo.identityserver.io/connect/token", "client", "secret", ssoServer.CreateHandler()))
            {
                var accessToken = Fake.ReferenceToken();
                ssoServer.Handle(async context =>
                {
                    await Task.Delay(10).ConfigureAwait(false); // NOTE: simulate network delay
                    context.Response = context.RequestIndex > 1
                        ? context.StatusCode((HttpStatusCode) 429) // NOTE: Too Many Requests
                        : context.Json(TokenConvert.ToJson(accessToken));
                });
                appServer.Handle(context =>
                {
                    var authorized = string.Equals(accessToken, context.Request.Headers.Authorization.Parameter);
                    context.Response = context.StatusCode(authorized ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
                });

                var authenticationCache = new AuthenticationCache(new ClientCredentialsProvider(tokenClient, "api"));
                await Task.WhenAll(Enumerable.Range(0, interactions).Select(async i =>
                {
                    await Task.Delay(1).ConfigureAwait(false);
                    using (var appClient = appServer.CreateClient(handler => new AuthenticateDelegatingHandler(authenticationCache, handler)))
                    {
                        await Task.WhenAll(Enumerable.Range(0, requests).Select(async j =>
                        {
                            // arrange
                            await Task.Delay(1).ConfigureAwait(false);

                            // act
                            var response = await appClient.GetAsync("https://demo.identityserver.io/api/test");

                            // assert
                            response.EnsureSuccessStatusCode();

                        })).ConfigureAwait(false);
                    }
                })).ConfigureAwait(false);
            }
        }

        private sealed class InProcAuthenticationCache : IAuthenticationCache
        {
            public string TokenType { get; set; } = "InProc";
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }

            public Task<bool> UpdateAsync(CancellationToken cancellationToken) => Task.FromResult(true);
            public Task<bool> RotateAsync(string accessToken, CancellationToken cancellationToken) => Task.FromResult(true);
        }
    }
}