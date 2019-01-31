// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class ResourceOwnerPasswordProviderTests
    {
        [Fact]
        public void Creating_with_token_client_should_not_throw()
        {
            // arrange
            var tokenClient = new TokenClient("http://server/token", "client");

            // act
            Action act = () => new ResourceOwnerPasswordProvider(tokenClient, "username", "password", "scope");

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Creating_should_not_throw()
        {
            // act
            Action act = () => new ResourceOwnerPasswordProvider("http://server/token", "client", "secret","username", "password", "scope");

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Dispose_should_not_throw()
        {
            // arrange
            var resourceOwnerPasswordProvider = new ResourceOwnerPasswordProvider("http://server/token", "client", "secret","username", "password", "scope");

            // act
            Action act = () => resourceOwnerPasswordProvider.Dispose();

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
                var userName = Fake.String();
                var password = Fake.String();
                var expectedAccessToken = Fake.ReferenceToken();
                var expectedRefreshToken = Fake.ReferenceToken();

                ssoServer.Handle(context =>
                {
                    if (context.RequestBody.Contains("grant_type=password") &&
                        context.RequestBody.Contains($"username={userName}") &&
                        context.RequestBody.Contains($"password={password}"))
                    {
                        context.Response = context.Json(TokenConvert.ToJson(expectedAccessToken, expectedRefreshToken));
                    }
                });

                var resourceOwnerPasswordProvider = new ResourceOwnerPasswordProvider(tokenClient, userName, password);

                // act
                var tokenResponse = await resourceOwnerPasswordProvider.RequestTokenAsync().ConfigureAwait(false);

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
                // arrange
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

                var resourceOwnerPasswordProvider = new ResourceOwnerPasswordProvider(tokenClient, Fake.String(), Fake.String());

                // act
                var tokenResponse = await resourceOwnerPasswordProvider.RefreshTokenAsync(refreshToken).ConfigureAwait(false);

                // assert
                tokenResponse.Should().NotBeNull();
                tokenResponse.AccessToken.Should().Be(expectedAccessToken);
                tokenResponse.RefreshToken.Should().Be(expectedRefreshToken);
            }
        }
    }
}