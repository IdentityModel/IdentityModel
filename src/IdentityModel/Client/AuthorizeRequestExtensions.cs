// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IdentityModel.Client
{
    /// <summary>
    /// Extensions for AuthorizeRequest
    /// </summary>
    public static class AuthorizeRequestExtensions
    {
        /// <summary>
        /// Creates the authorize URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="values">The values (either using a string Dictionary or an object's properties).</param>
        /// <returns></returns>
        public static string Create(this AuthorizeRequest request, object values)
        {
            return request.Create(ObjectToDictionary(values));
        }

        /// <summary>
        /// Creates the authorize URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="responseType">The response type.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="state">The state.</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="loginHint">The login hint.</param>
        /// <param name="acrValues">The acr values.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="responseMode">The response mode.</param>
        /// <param name="codeChallenge">The code challenge.</param>
        /// <param name="codeChallengeMethod">The code challenge method.</param>
        /// <param name="extra">Extra parameters.</param>
        /// <returns></returns>
        public static string CreateAuthorizeUrl(this AuthorizeRequest request,
            string clientId,
            string responseType,
            string scope = null,
            string redirectUri = null,
            string state = null,
            string nonce = null,
            string loginHint = null,
            string acrValues = null,
            string prompt = null,
            string responseMode = null,
            string codeChallenge = null,
            string codeChallengeMethod = null,
            object extra = null)
        {
            var values = new Dictionary<string, string>
            {
                { OidcConstants.AuthorizeRequest.ClientId, clientId },
                { OidcConstants.AuthorizeRequest.ResponseType, responseType }
            };

            if (!string.IsNullOrWhiteSpace(scope))
            {
                values.Add(OidcConstants.AuthorizeRequest.Scope, scope);
            }

            if (!string.IsNullOrWhiteSpace(redirectUri))
            {
                values.Add(OidcConstants.AuthorizeRequest.RedirectUri, redirectUri);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                values.Add(OidcConstants.AuthorizeRequest.State, state);
            }

            if (!string.IsNullOrWhiteSpace(nonce))
            {
                values.Add(OidcConstants.AuthorizeRequest.Nonce, nonce);
            }

            if (!string.IsNullOrWhiteSpace(loginHint))
            {
                values.Add(OidcConstants.AuthorizeRequest.LoginHint, loginHint);
            }

            if (!string.IsNullOrWhiteSpace(acrValues))
            {
                values.Add(OidcConstants.AuthorizeRequest.AcrValues, acrValues);
            }

            if (!string.IsNullOrWhiteSpace(prompt))
            {
                values.Add(OidcConstants.AuthorizeRequest.Prompt, prompt);
            }

            if (!string.IsNullOrWhiteSpace(responseMode))
            {
                values.Add(OidcConstants.AuthorizeRequest.ResponseMode, responseMode);
            }

            if (!string.IsNullOrWhiteSpace(codeChallenge))
            {
                values.Add(OidcConstants.AuthorizeRequest.CodeChallenge, codeChallenge);
            }

            if (!string.IsNullOrWhiteSpace(codeChallengeMethod))
            {
                values.Add(OidcConstants.AuthorizeRequest.CodeChallengeMethod, codeChallengeMethod);
            }

            return request.Create(Merge(values, ObjectToDictionary(extra)));
        }

        private static Dictionary<string, string> ObjectToDictionary(object values)
        {
            if (values == null)
            {
                return null;
            }

            if (values is Dictionary<string, string> dictionary) return dictionary;

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

        private static Dictionary<string, string> Merge(Dictionary<string, string> explicitValues, Dictionary<string, string> additionalValues = null)
        {
            var merged = explicitValues;

            if (additionalValues != null)
            {
                merged =
                    explicitValues.Concat(additionalValues.Where(add => !explicitValues.ContainsKey(add.Key)))
                                         .ToDictionary(final => final.Key, final => final.Value);
            }

            return merged;
        }
    }
}