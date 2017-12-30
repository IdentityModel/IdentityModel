// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IdentityModel.Client
{
    /// <summary>
    /// Extensions for RequestUrl
    /// </summary>
    public static class RequestUrlExtensions
    {
        /// <summary>
        /// Creates an authorize URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="values">The values (either using a string Dictionary or an object's properties).</param>
        /// <returns></returns>
        public static string Create(this RequestUrl request, object values)
        {
            return request.Create(ValuesHelper.ObjectToDictionary(values));
        }

        /// <summary>
        /// Creates an authorize URL.
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
        public static string CreateAuthorizeUrl(this RequestUrl request,
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

            values.AddIfPresent(OidcConstants.AuthorizeRequest.Scope, scope);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.RedirectUri, redirectUri);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.State, state);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.Nonce, nonce);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.LoginHint, loginHint);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.AcrValues, acrValues);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.Prompt, prompt);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.ResponseMode, responseMode);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.CodeChallenge, codeChallenge);
            values.AddIfPresent(OidcConstants.AuthorizeRequest.CodeChallengeMethod, codeChallengeMethod);

            return request.Create(ValuesHelper.Merge(values, ValuesHelper.ObjectToDictionary(extra)));
        }
    }
}