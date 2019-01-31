// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class ClientCredentialsProviderTests
    {
        [Fact]
        public void Creating_with_token_client_should_not_throw()
        {
            // arrange
            var tokenClient = new TokenClient("http://server/token", "client");

            // act
            Action act = () => new ClientCredentialsProvider(tokenClient, "scope");

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Creating_should_not_throw()
        {
            // act
            Action act = () => new ClientCredentialsProvider("http://server/token", "client", "secret", "scope");

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Dispose_should_not_throw()
        {
            // arrange
            var clientCredentialsProvider = new ClientCredentialsProvider("http://server/token", "client", "secret", "scope");

            // act
            Action act = () => clientCredentialsProvider.Dispose();

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public async Task Should_request_tokens()
        {
            using (var ssoServer = new InProcHttpServer())
            using (var tokenClient = new TokenClient("https://demo.identityserver.io/connect/token", "client", "secret", ssoServer.CreateHandler()))
            {
                // arrange
                var scope = Fake.String();
                var expectedAccessToken = Fake.ReferenceToken();
                var expectedRefreshToken = Fake.ReferenceToken();

                ssoServer.Handle(context =>
                {
                    if (context.RequestBody.Contains("grant_type=client_credentials") &&
                        context.RequestBody.Contains($"scope={scope}"))
                    {
                        context.Response = context.Json(TokenConvert.ToJson(expectedAccessToken, expectedRefreshToken));
                    }
                });

                var clientCredentialsProvider = new ClientCredentialsProvider(tokenClient, scope);

                // act
                var tokenResponse = await clientCredentialsProvider.RequestTokenAsync().ConfigureAwait(false);

                // assert
                tokenResponse.Should().NotBeNull();
                tokenResponse.AccessToken.Should().Be(expectedAccessToken);
                tokenResponse.RefreshToken.Should().Be(expectedRefreshToken);
            }
        }

        [Fact]
        public async Task Should_refresh_tokens()
        {
            using (var ssoServer = new InProcHttpServer())
            using (var tokenClient = new TokenClient("https://demo.identityserver.io/connect/token", "client", "secret", ssoServer.CreateHandler()))
            {
                var scope = Fake.String();
                var refreshToken = Fake.ReferenceToken();
                var expectedAccessToken = Fake.ReferenceToken();
                var expectedRefreshToken = Fake.ReferenceToken();

                ssoServer.Handle(context =>
                {
                    if (context.RequestBody.Contains("grant_type=refresh_token") &&
                        context.RequestBody.Contains($"refresh_token={refreshToken}"))
                    {
                        context.Response = context.Json(TokenConvert.ToJson(expectedAccessToken, expectedRefreshToken));
                    }
                });

                // arrange
                var clientCredentialsProvider = new ClientCredentialsProvider(tokenClient, scope);

                // act
                var tokenResponse = await clientCredentialsProvider.RefreshTokenAsync(refreshToken).ConfigureAwait(false);

                // assert
                tokenResponse.Should().NotBeNull();
                tokenResponse.AccessToken.Should().Be(expectedAccessToken);
                tokenResponse.RefreshToken.Should().Be(expectedRefreshToken);
            }
        }
    }
}