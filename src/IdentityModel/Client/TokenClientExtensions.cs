/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public static partial class TokenClientExtensions
    {
        public static Task<TokenResponse> RequestClientCredentialsAsync(this TokenClient client, string scope = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.ClientCredentials }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OAuth2Constants.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        public static Task<TokenResponse> RequestResourceOwnerPasswordAsync(this TokenClient client, string userName, string password, string scope = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.Password },
                { OAuth2Constants.UserName, userName },
                { OAuth2Constants.Password, password }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OAuth2Constants.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        public static Task<TokenResponse> RequestAuthorizationCodeAsync(this TokenClient client, string code, string redirectUri, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.AuthorizationCode },
                { OAuth2Constants.Code, code },
                { OAuth2Constants.RedirectUri, redirectUri }
            };

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        public static Task<TokenResponse> RequestRefreshTokenAsync(this TokenClient client, string refreshToken, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.RefreshToken },
                { OAuth2Constants.RefreshToken, refreshToken }
            };

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        public static Task<TokenResponse> RequestAssertionAsync(this TokenClient client, string assertionType, string assertion, string scope = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, assertionType },
                { OAuth2Constants.Assertion, assertion },
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OAuth2Constants.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        public static Task<TokenResponse> RequestCustomGrantAsync(this TokenClient client, string grantType, string scope = null, object extra = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fields = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, grantType }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                fields.Add(OAuth2Constants.Scope, scope);
            }

            return client.RequestAsync(Merge(client, fields, extra), cancellationToken);
        }

        public static Task<TokenResponse> RequestCustomAsync(this TokenClient client, object values, CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.RequestAsync(Merge(client, ObjectToDictionary(values)), cancellationToken);
        }

        private static Dictionary<string, string> Merge(TokenClient client, Dictionary<string, string> explicitValues, object extra = null)
        {
            var merged = explicitValues;

            if (client.AuthenticationStyle == AuthenticationStyle.PostValues)
            {
                merged.Add(OAuth2Constants.ClientId, client.ClientId);

                if (!string.IsNullOrEmpty(client.ClientSecret))
                {
                    merged.Add(OAuth2Constants.ClientSecret, client.ClientSecret);
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