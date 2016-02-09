// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityModel
{
    public static class OidcConstants
    {
        public static class AuthorizeRequest
        {
            public const string Scope               = "scope";
            public const string ResponseType        = "response_type";
            public const string ClientId            = "client_id";
            public const string RedirectUri         = "redirect_uri";
            public const string State               = "state";
            public const string ResponseMode        = "response_mode";
            public const string Nonce               = "nonce";
            public const string Display             = "display";
            public const string Prompt              = "prompt";
            public const string MaxAge              = "max_age";
            public const string UiLocales           = "ui_locales";
            public const string IdTokenHint         = "id_token_hint";
            public const string LoginHint           = "login_hint";
            public const string AcrValues           = "acr_values";
            public const string CodeChallenge       = "code_challenge";
            public const string CodeChallengeMethod = "code_challenge_method";
        }

        public static class AuthorizeErrors
        {
            // OAuth2 errors
            public const string InvalidRequest          = "invalid_request";
            public const string UnauthorizedClient      = "unauthorized_client";
            public const string AccessDenied            = "access_denied";
            public const string UnsupportedResponseType = "unsupported_response_type";
            public const string InvalidScope            = "invalid_scope";
            public const string ServerError             = "server_error";
            public const string TemporarilyUnavailable  = "temporarily_unavailable";

            // OIDC errors
            public const string InteractionRequired      = "interaction_required";
            public const string LoginRequired            = "login_required";
            public const string AccountSelectionRequired = "account_selection_required";
            public const string ConsentRequired          = "consent_required";
            public const string InvalidRequestUri        = "invalid_request_uri";
            public const string InvalidRequestObject     = "invalid_request_object";
            public const string RequestNotSupported      = "request_not_supported";
            public const string RequestUriNotSupported   = "request_uri_not_supported";
            public const string RegistrationNotSupported = "registration_not_supported";
        }

        public static class AuthorizeResponse
        {
            public const string Scope         = "scope";
            public const string Code          = "code";
            public const string AccessToken   = "access_token";
            public const string ExpiresIn     = "expires_in";
            public const string TokenType     = "token_type";
            public const string RefreshToken  = "refresh_token";
            public const string IdentityToken = "id_token";
            public const string State         = "state";
            public const string Error         = "error";
        }

        public static class TokenRequest
        {
            public const string GrantType           = "grant_type";
            public const string RedirectUri         = "redirect_uri";
            public const string ClientId            = "client_id";
            public const string ClientSecret        = "client_secret";
            public const string ClientAssertion     = "client_assertion";
            public const string ClientAssertionType = "client_assertion_type";
            public const string Assertion           = "assertion";
            public const string Code                = "code";
            public const string RefreshToken        = "refresh_token";
            public const string Scope               = "scope";
            public const string UserName            = "username";
            public const string Password            = "password";
            public const string CodeVerifier        = "code_verifier";
        }

        public static class TokenErrors
        {
            public const string InvalidRequest          = "invalid_request";
            public const string InvalidClient           = "invalid_client";
            public const string InvalidGrant            = "invalid_grant";
            public const string UnauthorizedClient      = "unauthorized_client";
            public const string UnsupportedGrantType    = "unsupported_grant_type";
            public const string UnsupportedResponseType = "unsupported_response_type";
            public const string InvalidScope            = "invalid_scope";
        }

        public static class TokenResponse
        {
            public const string AccessToken   = "access_token";
            public const string ExpiresIn     = "expires_in";
            public const string TokenType     = "token_type";
            public const string RefreshToken  = "refresh_token";
            public const string IdentityToken = "id_token";
            public const string Error         = "error";
        }

        public static class TokenTypes
        {
            public const string AccessToken   = "access_token";
            public const string IdentityToken = "id_token";
            public const string RefreshToken  = "refresh_token";
            public const string Bearer        = "Bearer";
        }

        public static class GrantTypes
        {
            public const string Password          = "password";
            public const string AuthorizationCode = "authorization_code";
            public const string ClientCredentials = "client_credentials";
            public const string RefreshToken      = "refresh_token";
            public const string Implicit          = "implicit";
            public const string Saml2Bearer       = "urn:ietf:params:oauth:grant-type:saml2-bearer";
            public const string JwtBearer         = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        }

        public static class ClientAssertionTypes
        {
            public const string JwtBearer = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
        }

        public static class ResponseTypes
        {
            public const string Code             = "code";
            public const string Token            = "token";
            public const string IdToken          = "id_token";
            public const string IdTokenToken     = "id_token token";
            public const string CodeIdToken      = "code id_token";
            public const string CodeToken        = "code token";
            public const string CodeIdTokenToken = "code id_token token";
        }

        public static class ResponseModes
        {
            public const string FormPost = "form_post";
            public const string Query    = "query";
            public const string Fragment = "fragment";
        }

        public static class DisplayModes
        {
            public const string Page  = "page";
            public const string Popup = "popup";
            public const string Touch = "touch";
            public const string Wap   = "wap";
        }

        public static class PromptModes
        {
            public const string None          = "none";
            public const string Login         = "login";
            public const string Consent       = "consent";
            public const string SelectAccount = "select_account";
        }

        public static class CodeChallengeMethods
        {
            public const string Plain  = "plain";
            public const string Sha256 = "S256";
        }

        public static class ProtectedResourceErrors
        {
            public const string InvalidToken      = "invalid_token";
            public const string ExpiredToken      = "expired_token";
            public const string InvalidRequest    = "invalid_request";
            public const string InsufficientScope = "insufficient_scope";
        }

        public static class EndpointAuthenticationMethods
        {
            public const string PostBody            = "client_secret_post";
            public const string BasicAuthentication = "client_secret_basic";
            public const string PrivateKeyJwt       = "private_key_jwt";
        }

        public static class AuthenticationMethods
        {
            public const string Password                        = "pwd";
            public const string ProofOfPossion                  = "pop";
            public const string OneTimePassword                 = "otp";
            public const string FingerprintBiometric            = "fpt";
            public const string RetinaScanBiometric             = "eye";
            public const string VoiceBiometric                  = "vbm";
            public const string ConfirmationByTelephone         = "tel";
            public const string ConfirmationBySms               = "sms";
            public const string KnowledgeBasedAuthentication    = "kba";
            public const string WindowsIntegratedAuthentication = "wia";
            public const string MultiFactorAuthentication       = "mfa";
            public const string UserPresenceTest                = "user";
            public const string RiskBasedAuthentication         = "risk";
            public const string MultipleChannelAuthentication   = "mfa";
        }
    }
}