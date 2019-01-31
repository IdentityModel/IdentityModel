// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class AuthenticationCacheTests
    {
        [Fact]
        public void Creating_should_not_throw()
        {
            // arrange
            var authenticationProvider = new InProcAuthenticationProvider();

            // act
            Action act = () => new AuthenticationCache(authenticationProvider);

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Dispose_should_not_throw()
        {
            // arrange
            var authenticationProvider = new InProcAuthenticationProvider();
            var authenticationCache = new AuthenticationCache(authenticationProvider);

            // act
            Action act = () => authenticationCache.Dispose();

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void TokenType_should_be_Bearer_by_default()
        {
            // arrange
            var authenticationProvider = new InProcAuthenticationProvider();

            // act
            var authenticationCache = new AuthenticationCache(authenticationProvider);

            // assert
            authenticationCache.TokenType.Should().Be("Bearer");
        }

        [Fact]
        public void Access_token_should_be_reusable()
        {
            // arrange
            var expectedAccessToken = Fake.ReferenceToken();

            var authenticationProvider = new InProcAuthenticationProvider();

            // act
            var authenticationCache = new AuthenticationCache(authenticationProvider, accessToken: expectedAccessToken);

            // assert
            authenticationCache.AccessToken.Should().Be(expectedAccessToken, "The access token should be reusable.");
        }

        [Fact]
        public void Refresh_token_should_be_reusable()
        {
            // arrange
            var expectedRefreshToken = Fake.ReferenceToken();

            var authenticationProvider = new InProcAuthenticationProvider();

            // act
            var authenticationCache = new AuthenticationCache(authenticationProvider, refreshToken: expectedRefreshToken);

            // assert
            authenticationCache.RefreshToken.Should().Be(expectedRefreshToken, "The refresh token should be reusable.");
        }

        [Fact]
        public async Task Refresh_token_should_be_retained_if_token_response_contains_only_access_token()
        {
            // arrange
            var oldAccessToken = Fake.ReferenceToken();
            var oldRefreshToken = Fake.ReferenceToken();
            var newAccessToken = Fake.ReferenceToken();

            var authenticationProvider = new InProcAuthenticationProvider(
                refreshToken: (_, ct) => Tuple.Create(newAccessToken, default(string))
            );
            var authenticationCache = new AuthenticationCache(authenticationProvider, oldRefreshToken, oldAccessToken);

            // act
            await authenticationCache.UpdateAsync().ConfigureAwait(false);

            // assert
            authenticationCache.AccessToken.Should().Be(newAccessToken);
            authenticationCache.RefreshToken.Should().Be(oldRefreshToken, "Refresh token should be retained if token response contains only access token");
        }

        [Fact]
        public async Task TokenReceived_should_be_raised_with_access_and_refresh_tokens()
        {
            // arrange
            var expectedAccessToken = Fake.ReferenceToken();
            var expectedRefreshToken = Fake.ReferenceToken();

            var authenticationProvider = new InProcAuthenticationProvider(
                acquireToken: _ => Tuple.Create(expectedAccessToken, expectedRefreshToken)
            );
            var authenticationCache = new AuthenticationCache(authenticationProvider);

            // act
            string accessToken = null, refreshToken = null;
            authenticationCache.TokenReceived += (sender, args) =>
            {
                accessToken = args.AccessToken;
                refreshToken = args.RefreshToken;
            };
            await authenticationCache.UpdateAsync().ConfigureAwait(false);

            // assert
            accessToken.Should().Be(expectedAccessToken);
            refreshToken.Should().Be(expectedRefreshToken);
        }

        [Fact]
        public async Task Update_should_request_tokens_depends_on_refresh_token()
        {
            string accessToken1 = Fake.ReferenceToken();
            string accessToken2 = Fake.ReferenceToken(), refreshToken2 = Fake.ReferenceToken();
            string accessToken3 = Fake.ReferenceToken(), refreshToken3 = Fake.ReferenceToken();

            using (var httpServer = new InProcHttpServer())
            using (var tokenClient = new TokenClient("http://server/token", "client", httpServer.CreateHandler()))
            {
                httpServer.Handle(context =>
                {
                    if (context.RequestBody.Contains("client_id=client"))
                    {
                        switch (context.RequestIndex)
                        {
                            case 1:
                            {
                                if (context.RequestBody.Contains("grant_type=client_credentials"))
                                {
                                    context.Response = context.Json(TokenConvert.ToJson(accessToken1));
                                }
                                break;
                            }
                            case 2:
                            {
                                if (context.RequestBody.Contains("grant_type=client_credentials"))
                                {
                                    context.Response = context.Json(TokenConvert.ToJson(accessToken2, refreshToken2));
                                }
                                break;
                            }
                            case 3:
                            {
                                if (context.RequestBody.Contains("grant_type=refresh_token") &&
                                    context.RequestBody.Contains($"refresh_token={refreshToken2}"))
                                {
                                    context.Response = context.Json(TokenConvert.ToJson(accessToken3, refreshToken3));
                                }
                                break;
                            }
                        }
                    }
                    context.Response = context.Response ?? context.StatusCode(HttpStatusCode.BadRequest);
                });

                // arrange
                var authenticationCache = new AuthenticationCache(new ClientCredentialsProvider(tokenClient));

                // act
                await authenticationCache.UpdateAsync().ConfigureAwait(false);

                // assert
                authenticationCache.AccessToken.Should().Be(accessToken1);
                authenticationCache.RefreshToken.Should().BeNull();

                // act
                await authenticationCache.UpdateAsync().ConfigureAwait(false);

                // assert
                authenticationCache.AccessToken.Should().Be(accessToken2);
                authenticationCache.RefreshToken.Should().Be(refreshToken2);

                // act
                await authenticationCache.UpdateAsync().ConfigureAwait(false);

                // assert
                authenticationCache.AccessToken.Should().Be(accessToken3);
                authenticationCache.RefreshToken.Should().Be(refreshToken3);
            }
        }

        [Fact]
        public async Task Rotate_should_do_nothing_if_access_tokens_are_not_equal()
        {
            // arrange
            var accessToken1 = Fake.ReferenceToken();
            var accessToken2 = Fake.ReferenceToken();

            var authenticationCache = new AuthenticationCache(new InProcAuthenticationProvider(), accessToken: accessToken2);

            // act
            var actual = await authenticationCache.RotateAsync(accessToken1).ConfigureAwait(false);

            // assert
            actual.Should().BeTrue("Access token should be retained if rotating access token differs.");
            authenticationCache.AccessToken.Should().Be(accessToken2);
        }

        [Fact]
        public async Task Rotate_should_do_nothing_if_timeout()
        {
            // arrange
            var countdown = new CountdownEvent(10);
            var authenticationProvider = new InProcAuthenticationProvider(acquireToken: _ =>
            {
                countdown.Wait(TimeSpan.FromSeconds(1));
                return Tuple.Create(Fake.ReferenceToken(), default(string));
            });
            var authenticationCache = new AuthenticationCache(authenticationProvider)
            {
                Timeout = TimeSpan.FromMilliseconds(1)
            };

            var accessToken = authenticationCache.AccessToken;

            // act
            var results = await Task.WhenAll(
                Enumerable.Range(0, countdown.InitialCount).Select(async _ =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                    var task = authenticationCache.RotateAsync(accessToken);
                    Task.Delay(TimeSpan.FromMilliseconds(25)).ContinueWith(x => countdown.Signal());
                    return await task.ConfigureAwait(false);
                })
            ).ConfigureAwait(false);

            // assert
            results.Count(_ => _).Should().Be(1, "Only 1 operation succeeded, the rest skipped by timeout.");
        }

        [Fact]
        public async Task Rotate_should_update_tokens_if_access_tokens_are_equal()
        {
            // arrange
            var accessToken1 = Fake.ReferenceToken();
            var accessToken2 = Fake.ReferenceToken();

            var authenticationProvider = new InProcAuthenticationProvider(
                acquireToken: _ => Tuple.Create(accessToken2, default(string))
            );
            var authenticationCache = new AuthenticationCache(authenticationProvider, accessToken: accessToken1);

            // act
            var actual = await authenticationCache.RotateAsync(accessToken1).ConfigureAwait(false);

            // assert
            actual.Should().BeTrue("Access token should be rotated if access tokens are equal.");
            authenticationCache.AccessToken.Should().Be(accessToken2);
        }

        [Fact]
        public async Task Rotate_should_update_tokens_only_once_during_the_same_rotation()
        {
            // arrange
            var accessToken = Fake.ReferenceToken();
            var refreshToken = Fake.ReferenceToken();

            var authenticationProvider = new InProcAuthenticationProvider(refreshToken: (token, _) =>
            {
                if (token == refreshToken)
                {
                    return Tuple.Create(Fake.ReferenceToken(), Fake.ReferenceToken());
                }
                throw new NotSupportedException("Access token should be requested once.");
            });
            var authenticationCache = new AuthenticationCache(authenticationProvider, refreshToken, accessToken);

            // act
            var results = await Task.WhenAll(
                Enumerable.Range(0, 10).Select(async _ =>
                {
                    await Task.Delay(1).ConfigureAwait(false);
                    return await authenticationCache.RotateAsync(accessToken).ConfigureAwait(false);
                })
            ).ConfigureAwait(false);

            // assert
            results.Should().AllBeEquivalentTo(true);
        }

        private sealed class InProcAuthenticationProvider : IAuthenticationProvider
        {
            private readonly Func<CancellationToken, Tuple<string, string>> _acquireToken;
            private readonly Func<string, CancellationToken, Tuple<string, string>> _refreshToken;

            public InProcAuthenticationProvider(
                Func<CancellationToken, Tuple<string, string>> acquireToken = null,
                Func<string, CancellationToken, Tuple<string, string>> refreshToken = null)
            {
                _acquireToken = acquireToken;
                _refreshToken = refreshToken;
            }

            public async Task<TokenResponse> RequestTokenAsync(CancellationToken cancellationToken)
            {
                if (_acquireToken == null)
                {
                    return new TokenResponse(new NotSupportedException());
                }

                var (newAccessToken, newRefreshToken) = _acquireToken.Invoke(cancellationToken);
                return TokenConvert.ToTokenResponse(newAccessToken, newRefreshToken);
            }

            public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
            {
                if (_refreshToken == null)
                {
                    return new TokenResponse(new NotSupportedException());
                }

                var (newAccessToken, newRefreshToken) = _refreshToken.Invoke(refreshToken, cancellationToken);
                return TokenConvert.ToTokenResponse(newAccessToken, newRefreshToken);
            }
        }
    }
}