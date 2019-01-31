// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class AuthorizationCodeProviderTests
    {
        [Fact]
        public void Creating_with_token_client_should_not_throw()
        {
            // arrange
            var tokenClient = new TokenClient("http://server/token", "client");

            // act
            Action act = () => new AuthorizationCodeProvider(tokenClient, "code", "redirectUrl");

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Creating_should_not_throw()
        {
            // act
            Action act = () => new AuthorizationCodeProvider("http://server/token", "client", "secret", "code", "redirectUrl");

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Dispose_should_not_throw()
        {
            // arrange
            var authorizationCodeProvider = new AuthorizationCodeProvider("http://server/token", "client", "secret", "code", "redirectUrl");

            // act
            Action act = () => authorizationCodeProvider.Dispose();

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
                var code = Fake.String();
                var redirectUri = Fake.String();
                var expectedAccessToken = Fake.ReferenceToken();
                var expectedRefreshToken = Fake.ReferenceToken();

                ssoServer.Handle(context =>
                {
                    if (context.RequestBody.Contains("grant_type=authorization_code") &&
                        context.RequestBody.Contains($"code={code}") &&
                        context.RequestBody.Contains($"redirect_uri={redirectUri}"))
                    {
                        context.Response = context.Json(TokenConvert.ToJson(expectedAccessToken, expectedRefreshToken));
                    }
                });

                var authorizationCodeProvider = new AuthorizationCodeProvider(tokenClient, code, redirectUri);

                // act
                var tokenResponse = await authorizationCodeProvider.RequestTokenAsync().ConfigureAwait(false);

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

                var authorizationCodeProvider = new AuthorizationCodeProvider(tokenClient, Fake.String(), Fake.String());

                // act
                var tokenResponse = await authorizationCodeProvider.RefreshTokenAsync(refreshToken).ConfigureAwait(false);

                // assert
                tokenResponse.Should().NotBeNull();
                tokenResponse.AccessToken.Should().Be(expectedAccessToken);
                tokenResponse.RefreshToken.Should().Be(expectedRefreshToken);
            }
        }
    }
}