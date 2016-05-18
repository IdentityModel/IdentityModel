// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace IdentityModel.Client
{
    [Obsolete("Use OidcConstants instead. Will be removed in next major version.")]
    public static class OAuth2Constants
    {
        public const string GrantType = "grant_type";
        public const string UserName = "username";
        public const string Scope = "scope";
        public const string Assertion = "assertion";
        public const string Password = "password";
        public const string Code = "code";
        public const string RedirectUri = "redirect_uri";
        public const string AccessToken = "access_token";
        public const string ExpiresIn = "expires_in";
        public const string TokenType = "token_type";
        public const string RefreshToken = "refresh_token";
        public const string IdentityToken = "id_token";
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string ResponseType = "response_type";
        public const string State = "state";
        public const string Nonce = "nonce";
        public const string LoginHint = "login_hint";
        public const string AcrValues = "acr_values";
        public const string Error = "error";
        public const string ResponseMode = "response_mode";

        public static class GrantTypes
        {
            public const string Password = "password";
            public const string AuthorizationCode = "authorization_code";
            public const string ClientCredentials = "client_credentials";
            public const string RefreshToken = "refresh_token";
            public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
            public const string Saml2Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";
        }

        public static class ResponseTypes
        {
            public const string Token = "token";
            public const string Code = "code";
        }

        public static class Errors
        {
            public const string Error = "error";
            public const string InvalidRequest = "invalid_request";
            public const string InvalidClient = "invalid_client";
            public const string InvalidGrant = "invalid_grant";
            public const string UnauthorizedClient = "unauthorized_client";
            public const string UnsupportedGrantType = "unsupported_grant_type";
            public const string UnsupportedResponseType = "unsupported_response_type";
            public const string InvalidScope = "invalid_scope";
            public const string AccessDenied = "access_denied";
        }
    }
}