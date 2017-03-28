// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Extensions for TokenClient
    /// </summary>
    public static class TokenClientExtensions
    {
        /// <summary>
        /// Requests a token based on client credentials.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="extra">Eextra parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestClientCredentialsAsync(this TokenClient client, string scope = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.ClientCredentials }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OidcConstants.TokenRequest.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        /// <summary>
        /// Requests a token using the resource owner password credentials.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="extra">Extra parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestResourceOwnerPasswordAsync(this TokenClient client, string userName, string password, string scope = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password },
                { OidcConstants.TokenRequest.UserName, userName },
                { OidcConstants.TokenRequest.Password, password }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OidcConstants.TokenRequest.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        /// <summary>
        /// Requests a token using an authorization code.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="code">The code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="codeVerifier">The code verifier.</param>
        /// <param name="extra">Extra parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestAuthorizationCodeAsync(this TokenClient client, string code, string redirectUri, string codeVerifier = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.AuthorizationCode },
                { OidcConstants.TokenRequest.Code, code },
                { OidcConstants.TokenRequest.RedirectUri, redirectUri }
            };

            if (!string.IsNullOrWhiteSpace(codeVerifier))
            {
                fields.Add(OidcConstants.TokenRequest.CodeVerifier, codeVerifier);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        /// <summary>
        /// Requests a PoP token using an authorization code.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="code">The code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="codeVerifier">The code verifier.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="key">The key.</param>
        /// <param name="extra">Extra parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestAuthorizationCodePopAsync(this TokenClient client, string code, string redirectUri, string codeVerifier = null, string algorithm = null, string key = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OidcConstants.TokenRequest.TokenType, OidcConstants.TokenRequestTypes.Pop  },
                { OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.AuthorizationCode },
                { OidcConstants.TokenRequest.Code, code },
                { OidcConstants.TokenRequest.RedirectUri, redirectUri }
            };

            if (!string.IsNullOrWhiteSpace(codeVerifier))
            {
                fields.Add(OidcConstants.TokenRequest.CodeVerifier, codeVerifier);
            }

            if (!string.IsNullOrWhiteSpace(algorithm))
            {
                fields.Add(OidcConstants.TokenRequest.Algorithm, algorithm);
            }

            if (!string.IsNullOrWhiteSpace(key))
            {
                fields.Add(OidcConstants.TokenRequest.Key, key);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        /// <summary>
        /// Requests a token using a refresh token.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="extra">Extra parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestRefreshTokenAsync(this TokenClient client, string refreshToken, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken },
                { OidcConstants.TokenRequest.RefreshToken, refreshToken }
            };

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        /// <summary>
        /// Requests a PoP token using a refresh token.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="key">The key.</param>
        /// <param name="extra">Extra parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestRefreshTokenPopAsync(this TokenClient client, string refreshToken, string algorithm = null, string key = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OidcConstants.TokenRequest.TokenType, OidcConstants.TokenRequestTypes.Pop  },
                { OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken },
                { OidcConstants.TokenRequest.RefreshToken, refreshToken }
            };

            if (!string.IsNullOrWhiteSpace(algorithm))
            {
                fields.Add(OidcConstants.TokenRequest.Algorithm, algorithm);
            }

            if (!string.IsNullOrWhiteSpace(key))
            {
                fields.Add(OidcConstants.TokenRequest.Key, key);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        /// <summary>
        /// Requests a token using a an assertion.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="assertionType">Type of the assertion.</param>
        /// <param name="assertion">The assertion.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="extra">Extra parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestAssertionAsync(this TokenClient client, string assertionType, string assertion, string scope = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OidcConstants.TokenRequest.GrantType, assertionType },
                { OidcConstants.TokenRequest.Assertion, assertion },
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OidcConstants.TokenRequest.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        /// <summary>
        /// Requests a token using a custom grant.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="grantType">Type of the grant.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="extra">Extra parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestCustomGrantAsync(this TokenClient client, string grantType, string scope = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OidcConstants.TokenRequest.GrantType, grantType }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OidcConstants.TokenRequest.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        /// <summary>
        /// Requests a token using a custom request
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="values">The values.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static Task<TokenResponse> RequestCustomAsync(this TokenClient client, object values, CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.RequestAsync(Merge(client, ObjectToDictionary(values)), cancellationToken);
        }

        private static Dictionary<string, string> Merge(TokenClient client, Dictionary<string, string> explicitValues, object extra = null)
        {
            var merged = explicitValues;

            if (client.AuthenticationStyle == AuthenticationStyle.PostValues)
            {
                merged.Add(OidcConstants.TokenRequest.ClientId, client.ClientId);

                if (!string.IsNullOrEmpty(client.ClientSecret))
                {
                    merged.Add(OidcConstants.TokenRequest.ClientSecret, client.ClientSecret);
                }
            }

            var additionalValues = ObjectToDictionary(extra);

            if (additionalValues != null)
            {
                merged =
                    explicitValues.Concat(additionalValues.Where(add => !explicitValues.ContainsKey(add.Key)))
                                         .ToDictionary(final => final.Key, final => final.Value);
            }

            return merged;
        }

        private static Dictionary<string, string> ObjectToDictionary(object values)
        {
            if (values == null)
            {
                return null;
            }

            var dictionary = values as Dictionary<string, string>;
            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<string, string>();

            foreach (var prop in values.GetType().GetRuntimeProperties())
            {
                var value = prop.GetValue(values) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    dictionary.Add(prop.Name, value);
                }
            }

            return dictionary;
        }
    }
}