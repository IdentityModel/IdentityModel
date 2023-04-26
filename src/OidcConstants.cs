// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#pragma warning disable 1591

namespace IdentityModel;

public static class OidcConstants
{
    public static class AuthorizeRequest
    {
        public const string Scope = "scope";
        public const string ResponseType = "response_type";
        public const string ClientId = "client_id";
        public const string RedirectUri = "redirect_uri";
        public const string State = "state";
        public const string ResponseMode = "response_mode";
        public const string Nonce = "nonce";
        public const string Display = "display";
        public const string Prompt = "prompt";
        public const string MaxAge = "max_age";
        public const string UiLocales = "ui_locales";
        public const string IdTokenHint = "id_token_hint";
        public const string LoginHint = "login_hint";
        public const string AcrValues = "acr_values";
        public const string CodeChallenge = "code_challenge";
        public const string CodeChallengeMethod = "code_challenge_method";
        public const string Request = "request";
        public const string RequestUri = "request_uri";
        public const string Resource = "resource";
        public const string DPoPKeyThumbprint = "dpop_jkt";
    }

    public static class AuthorizeErrors
    {
        // OAuth2 errors
        public const string InvalidRequest = "invalid_request";
        public const string UnauthorizedClient = "unauthorized_client";
        public const string AccessDenied = "access_denied";
        public const string UnsupportedResponseType = "unsupported_response_type";
        public const string InvalidScope = "invalid_scope";
        public const string ServerError = "server_error";
        public const string TemporarilyUnavailable = "temporarily_unavailable";
        public const string UnmetAuthenticationRequirements = "unmet_authentication_requirements";

        // OIDC errors
        public const string InteractionRequired = "interaction_required";
        public const string LoginRequired = "login_required";
        public const string AccountSelectionRequired = "account_selection_required";
        public const string ConsentRequired = "consent_required";
        public const string InvalidRequestUri = "invalid_request_uri";
        public const string InvalidRequestObject = "invalid_request_object";
        public const string RequestNotSupported = "request_not_supported";
        public const string RequestUriNotSupported = "request_uri_not_supported";
        public const string RegistrationNotSupported = "registration_not_supported";

        // resource indicator spec
        public const string InvalidTarget = "invalid_target";
    }

    public static class AuthorizeResponse
    {
        public const string Scope = "scope";
        public const string Code = "code";
        public const string AccessToken = "access_token";
        public const string ExpiresIn = "expires_in";
        public const string TokenType = "token_type";
        public const string RefreshToken = "refresh_token";
        public const string IdentityToken = "id_token";
        public const string State = "state";
        public const string SessionState = "session_state";
        public const string Issuer = "iss";
        public const string Error = "error";
        public const string ErrorDescription = "error_description";
    }

    public static class DeviceAuthorizationResponse
    {
        public const string DeviceCode = "device_code";
        public const string UserCode = "user_code";
        public const string VerificationUri = "verification_uri";
        public const string VerificationUriComplete = "verification_uri_complete";
        public const string ExpiresIn = "expires_in";
        public const string Interval = "interval";
    }

    public static class EndSessionRequest
    {
        public const string IdTokenHint = "id_token_hint";
        public const string PostLogoutRedirectUri = "post_logout_redirect_uri";
        public const string State = "state";
        public const string Sid = "sid";
        public const string Issuer = "iss";
        public const string UiLocales = "ui_locales";
    }

    public static class TokenRequest
    {
        public const string GrantType = "grant_type";
        public const string RedirectUri = "redirect_uri";
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string ClientAssertion = "client_assertion";
        public const string ClientAssertionType = "client_assertion_type";
        public const string Assertion = "assertion";
        public const string Code = "code";
        public const string RefreshToken = "refresh_token";
        public const string Scope = "scope";
        public const string UserName = "username";
        public const string Password = "password";
        public const string CodeVerifier = "code_verifier";
        public const string TokenType = "token_type";
        public const string Algorithm = "alg";
        public const string Key = "key";
        public const string DeviceCode = "device_code";

        // token exchange
        public const string Resource = "resource";
        public const string Audience = "audience";
        public const string RequestedTokenType = "requested_token_type";
        public const string SubjectToken = "subject_token";
        public const string SubjectTokenType = "subject_token_type";
        public const string ActorToken = "actor_token";
        public const string ActorTokenType = "actor_token_type";
        
        // ciba
        public const string AuthenticationRequestId = "auth_req_id";
    }

    public static class BackchannelAuthenticationRequest
    {
        public const string Scope = "scope";
        public const string ClientNotificationToken = "client_notification_token";
        public const string AcrValues = "acr_values";
        public const string LoginHintToken = "login_hint_token";
        public const string IdTokenHint = "id_token_hint";
        public const string LoginHint = "login_hint";
        public const string BindingMessage = "binding_message";
        public const string UserCode = "user_code";
        public const string RequestedExpiry = "requested_expiry";
        public const string Request = "request";
        public const string Resource = "resource";
        public const string DPoPKeyThumbprint = "dpop_jkt";
    }

    public static class BackchannelAuthenticationRequestErrors
    {
        public const string InvalidRequestObject = "invalid_request_object";
        public const string InvalidRequest = "invalid_request";
        public const string InvalidScope = "invalid_scope";
        public const string ExpiredLoginHintToken = "expired_login_hint_token";
        public const string UnknownUserId = "unknown_user_id";
        public const string UnauthorizedClient = "unauthorized_client";
        public const string MissingUserCode = "missing_user_code";
        public const string InvalidUserCode = "invalid_user_code";
        public const string InvalidBindingMessage = "invalid_binding_message";
        public const string InvalidClient = "invalid_client";
        public const string AccessDenied = "access_denied";
        public const string InvalidTarget = "invalid_target";
    }

    public static class TokenRequestTypes
    {
        public const string Bearer = "bearer";
        public const string Pop = "pop";
    }

    public static class TokenErrors
    {
        public const string InvalidRequest = "invalid_request";
        public const string InvalidClient = "invalid_client";
        public const string InvalidGrant = "invalid_grant";
        public const string UnauthorizedClient = "unauthorized_client";
        public const string UnsupportedGrantType = "unsupported_grant_type";
        public const string UnsupportedResponseType = "unsupported_response_type";
        public const string InvalidScope = "invalid_scope";
        public const string AuthorizationPending = "authorization_pending";
        public const string AccessDenied = "access_denied";
        public const string SlowDown = "slow_down";
        public const string ExpiredToken = "expired_token";
        public const string InvalidTarget = "invalid_target";
        public const string InvalidDPoPProof = "invalid_dpop_proof";
        public const string UseDPoPNonce = "use_dpop_nonce";
    }

    public static class TokenResponse
    {
        public const string AccessToken = "access_token";
        public const string ExpiresIn = "expires_in";
        public const string TokenType = "token_type";
        public const string RefreshToken = "refresh_token";
        public const string IdentityToken = "id_token";
        public const string Error = "error";
        public const string ErrorDescription = "error_description";
        public const string BearerTokenType = "Bearer";
        public const string DPoPTokenType = "DPoP";
        public const string IssuedTokenType = "issued_token_type";
        public const string Scope = "scope";
    }

    public static class BackchannelAuthenticationResponse
    {
        public const string AuthenticationRequestId = "auth_req_id";
        public const string ExpiresIn = "expires_in";
        public const string Interval = "interval";
    }

    public static class TokenIntrospectionRequest
    {
        public const string Token = "token";
        public const string TokenTypeHint = "token_type_hint";
    }

    public static class RegistrationResponse
    {
        public const string Error = "error";
        public const string ErrorDescription = "error_description";
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string RegistrationAccessToken = "registration_access_token";
        public const string RegistrationClientUri = "registration_client_uri";
        public const string ClientIdIssuedAt = "client_id_issued_at";
        public const string ClientSecretExpiresAt = "client_secret_expires_at";
        public const string SoftwareStatement = "software_statement";
    }

    public static class ClientMetadata
    {
        public const string RedirectUris = "redirect_uris";
        public const string ResponseTypes = "response_types";
        public const string GrantTypes = "grant_types";
        public const string ApplicationType = "application_type";
        public const string Contacts = "contacts";
        public const string ClientName = "client_name";
        public const string LogoUri = "logo_uri";
        public const string ClientUri = "client_uri";
        public const string PolicyUri = "policy_uri";
        public const string TosUri = "tos_uri";
        public const string JwksUri = "jwks_uri";
        public const string Jwks = "jwks";
        public const string SectorIdentifierUri = "sector_identifier_uri";
        public const string Scope = "scope";
        public const string PostLogoutRedirectUris = "post_logout_redirect_uris";
        public const string FrontChannelLogoutUri = "frontchannel_logout_uri";
        public const string FrontChannelLogoutSessionRequired = "frontchannel_logout_session_required";
        public const string BackchannelLogoutUri = "backchannel_logout_uri";
        public const string BackchannelLogoutSessionRequired = "backchannel_logout_session_required";
        public const string SoftwareId = "software_id";
        public const string SoftwareStatement = "software_statement";
        public const string SoftwareVersion = "software_version";
        public const string SubjectType = "subject_type";
        public const string TokenEndpointAuthenticationMethod = "token_endpoint_auth_method";
        public const string TokenEndpointAuthenticationSigningAlgorithm = "token_endpoint_auth_signing_alg";
        public const string DefaultMaxAge = "default_max_age";
        public const string RequireAuthenticationTime = "require_auth_time";
        public const string DefaultAcrValues = "default_acr_values";
        public const string InitiateLoginUri = "initiate_login_uri";
        public const string RequestUris = "request_uris";
        public const string IdentityTokenSignedResponseAlgorithm = "id_token_signed_response_alg";
        public const string IdentityTokenEncryptedResponseAlgorithm = "id_token_encrypted_response_alg";
        public const string IdentityTokenEncryptedResponseEncryption = "id_token_encrypted_response_enc";
        public const string UserinfoSignedResponseAlgorithm = "userinfo_signed_response_alg";
        public const string UserInfoEncryptedResponseAlgorithm = "userinfo_encrypted_response_alg";
        public const string UserinfoEncryptedResponseEncryption = "userinfo_encrypted_response_enc";
        public const string RequestObjectSigningAlgorithm = "request_object_signing_alg";
        public const string RequestObjectEncryptionAlgorithm = "request_object_encryption_alg";
        public const string RequestObjectEncryptionEncryption = "request_object_encryption_enc";
        public const string RequireSignedRequestObject = "require_signed_request_object";
        public const string AlwaysUseDPoPBoundAccessTokens = "dpop_bound_access_tokens";
    }

    public static class TokenTypes
    {
        public const string AccessToken = "access_token";
        public const string IdentityToken = "id_token";
        public const string RefreshToken = "refresh_token";
    }

    public static class TokenTypeIdentifiers
    {
        public const string AccessToken = "urn:ietf:params:oauth:token-type:access_token";
        public const string IdentityToken = "urn:ietf:params:oauth:token-type:id_token";
        public const string RefreshToken = "urn:ietf:params:oauth:token-type:refresh_token";
        public const string Saml11 = "urn:ietf:params:oauth:token-type:saml1";
        public const string Saml2 = "urn:ietf:params:oauth:token-type:saml2";
        public const string Jwt = "urn:ietf:params:oauth:token-type:jwt";
    }

    public static class AuthenticationSchemes
    {
        public const string AuthorizationHeaderBearer = "Bearer";
        public const string AuthorizationHeaderDPoP = "DPoP";
        
        public const string FormPostBearer = "access_token";
        public const string QueryStringBearer = "access_token";

        public const string AuthorizationHeaderPop = "PoP";
        public const string FormPostPop = "pop_access_token";
        public const string QueryStringPop = "pop_access_token";
    }

    public static class GrantTypes
    {
        public const string Password = "password";
        public const string AuthorizationCode = "authorization_code";
        public const string ClientCredentials = "client_credentials";
        public const string RefreshToken = "refresh_token";
        public const string Implicit = "implicit";
        public const string Saml2Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";
        public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        public const string DeviceCode = "urn:ietf:params:oauth:grant-type:device_code";
        public const string TokenExchange = "urn:ietf:params:oauth:grant-type:token-exchange";
        public const string Ciba = "urn:openid:params:grant-type:ciba";
    }

    public static class ClientAssertionTypes
    {
        public const string JwtBearer = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
        public const string SamlBearer = "urn:ietf:params:oauth:client-assertion-type:saml2-bearer";
    }

    public static class ResponseTypes
    {
        public const string Code = "code";
        public const string Token = "token";
        public const string IdToken = "id_token";
        public const string IdTokenToken = "id_token token";
        public const string CodeIdToken = "code id_token";
        public const string CodeToken = "code token";
        public const string CodeIdTokenToken = "code id_token token";
    }

    public static class ResponseModes
    {
        public const string FormPost = "form_post";
        public const string Query = "query";
        public const string Fragment = "fragment";
    }

    public static class DisplayModes
    {
        public const string Page = "page";
        public const string Popup = "popup";
        public const string Touch = "touch";
        public const string Wap = "wap";
    }

    public static class PromptModes
    {
        public const string None = "none";
        public const string Login = "login";
        public const string Consent = "consent";
        public const string SelectAccount = "select_account";
        public const string Create = "create";
    }

    public static class CodeChallengeMethods
    {
        public const string Plain = "plain";
        public const string Sha256 = "S256";
    }

    public static class ProtectedResourceErrors
    {
        public const string InvalidToken = "invalid_token";
        public const string ExpiredToken = "expired_token";
        public const string InvalidRequest = "invalid_request";
        public const string InsufficientScope = "insufficient_scope";
    }

    public static class EndpointAuthenticationMethods
    {
        public const string PostBody = "client_secret_post";
        public const string BasicAuthentication = "client_secret_basic";
        public const string PrivateKeyJwt = "private_key_jwt";
        public const string TlsClientAuth = "tls_client_auth";
        public const string SelfSignedTlsClientAuth = "self_signed_tls_client_auth";
    }

    public static class AuthenticationMethods
    {
        public const string FacialRecognition = "face";
        public const string FingerprintBiometric = "fpt";
        public const string Geolocation = "geo";
        public const string ProofOfPossessionHardwareSecuredKey = "hwk";
        public const string IrisScanBiometric = "iris";
        public const string KnowledgeBasedAuthentication = "kba";
        public const string MultipleChannelAuthentication = "mca";
        public const string MultiFactorAuthentication = "mfa";
        public const string OneTimePassword = "otp";
        public const string PersonalIdentificationOrPattern = "pin";
        public const string Password = "pwd";
        public const string RiskBasedAuthentication = "rba";
        public const string RetinaScanBiometric = "retina";
        public const string SmartCard = "sc";
        public const string ConfirmationBySms = "sms";
        public const string ProofOfPossessionSoftwareSecuredKey = "swk";
        public const string ConfirmationByTelephone = "tel";
        public const string UserPresenceTest = "user";
        public const string VoiceBiometric = "vbm";
        public const string WindowsIntegratedAuthentication = "wia";
    }

    public static class Algorithms
    {
        public const string None = "none";

        public static class Symmetric
        {
            public const string HS256 = "HS256";
            public const string HS384 = "HS284";
            public const string HS512 = "HS512";
        }

        public static class Asymmetric
        {
            public const string RS256 = "RS256";
            public const string RS384 = "RS384";
            public const string RS512 = "RS512";

            public const string ES256 = "ES256";
            public const string ES384 = "ES384";
            public const string ES512 = "ES512";

            public const string PS256 = "PS256";
            public const string PS384 = "PS384";
            public const string PS512 = "PS512";

        }
    }

    public static class Discovery
    {
        public const string Issuer = "issuer";

        // endpoints
        public const string AuthorizationEndpoint = "authorization_endpoint";
        public const string DeviceAuthorizationEndpoint = "device_authorization_endpoint";
        public const string TokenEndpoint = "token_endpoint";
        public const string UserInfoEndpoint = "userinfo_endpoint";
        public const string IntrospectionEndpoint = "introspection_endpoint";
        public const string RevocationEndpoint = "revocation_endpoint";
        public const string DiscoveryEndpoint = ".well-known/openid-configuration";
        public const string JwksUri = "jwks_uri";
        public const string EndSessionEndpoint = "end_session_endpoint";
        public const string CheckSessionIframe = "check_session_iframe";
        public const string RegistrationEndpoint = "registration_endpoint";
        public const string MtlsEndpointAliases = "mtls_endpoint_aliases";

        // common capabilities
        public const string FrontChannelLogoutSupported = "frontchannel_logout_supported";
        public const string FrontChannelLogoutSessionSupported = "frontchannel_logout_session_supported";
        public const string BackChannelLogoutSupported = "backchannel_logout_supported";
        public const string BackChannelLogoutSessionSupported = "backchannel_logout_session_supported";
        public const string GrantTypesSupported = "grant_types_supported";
        public const string CodeChallengeMethodsSupported = "code_challenge_methods_supported";
        public const string ScopesSupported = "scopes_supported";
        public const string SubjectTypesSupported = "subject_types_supported";
        public const string ResponseModesSupported = "response_modes_supported";
        public const string ResponseTypesSupported = "response_types_supported";
        public const string ClaimsSupported = "claims_supported";
        public const string TokenEndpointAuthenticationMethodsSupported = "token_endpoint_auth_methods_supported";

        // more capabilities
        public const string ClaimsLocalesSupported = "claims_locales_supported";
        public const string ClaimsParameterSupported = "claims_parameter_supported";
        public const string ClaimTypesSupported = "claim_types_supported";
        public const string DisplayValuesSupported = "display_values_supported";
        public const string AcrValuesSupported = "acr_values_supported";
        public const string IdTokenEncryptionAlgorithmsSupported = "id_token_encryption_alg_values_supported";
        public const string IdTokenEncryptionEncValuesSupported = "id_token_encryption_enc_values_supported";
        public const string IdTokenSigningAlgorithmsSupported = "id_token_signing_alg_values_supported";
        public const string OpPolicyUri = "op_policy_uri";
        public const string OpTosUri = "op_tos_uri";
        public const string RequestObjectEncryptionAlgorithmsSupported = "request_object_encryption_alg_values_supported";
        public const string RequestObjectEncryptionEncValuesSupported = "request_object_encryption_enc_values_supported";
        public const string RequestObjectSigningAlgorithmsSupported = "request_object_signing_alg_values_supported";
        public const string RequestParameterSupported = "request_parameter_supported";
        public const string RequestUriParameterSupported = "request_uri_parameter_supported";
        public const string RequireRequestUriRegistration = "require_request_uri_registration";
        public const string ServiceDocumentation = "service_documentation";
        public const string TokenEndpointAuthSigningAlgorithmsSupported = "token_endpoint_auth_signing_alg_values_supported";
        public const string UILocalesSupported = "ui_locales_supported";
        public const string UserInfoEncryptionAlgorithmsSupported = "userinfo_encryption_alg_values_supported";
        public const string UserInfoEncryptionEncValuesSupported = "userinfo_encryption_enc_values_supported";
        public const string UserInfoSigningAlgorithmsSupported = "userinfo_signing_alg_values_supported";
        public const string TlsClientCertificateBoundAccessTokens = "tls_client_certificate_bound_access_tokens";
        public const string AuthorizationResponseIssParameterSupported = "authorization_response_iss_parameter_supported";
        public const string PromptValuesSupported = "prompt_values_supported";

        // CIBA
        public const string BackchannelTokenDeliveryModesSupported = "backchannel_token_delivery_modes_supported";
        public const string BackchannelAuthenticationEndpoint = "backchannel_authentication_endpoint";
        public const string BackchannelAuthenticationRequestSigningAlgValuesSupported = "backchannel_authentication_request_signing_alg_values_supported";
        public const string BackchannelUserCodeParameterSupported = "backchannel_user_code_parameter_supported";
        
        // DPoP
        public const string DPoPSigningAlgorithmsSupported = "dpop_signing_alg_values_supported";
    }

    public static class BackchannelTokenDeliveryModes
    {
        public const string Poll = "poll";
        public const string Ping = "ping";
        public const string Push = "push";
    }

    public static class Events
    {
        public const string BackChannelLogout = "http://schemas.openid.net/event/backchannel-logout";
    }

    public static class BackChannelLogoutRequest
    {
        public const string LogoutToken = "logout_token";
    }

    public static class StandardScopes
    {
        /// <summary>REQUIRED. Informs the Authorization Server that the Client is making an OpenID Connect request. If the <c>openid</c> scope value is not present, the behavior is entirely unspecified.</summary>
        public const string OpenId = "openid";
        /// <summary>OPTIONAL. This scope value requests access to the End-User's default profile Claims, which are: <c>name</c>, <c>family_name</c>, <c>given_name</c>, <c>middle_name</c>, <c>nickname</c>, <c>preferred_username</c>, <c>profile</c>, <c>picture</c>, <c>website</c>, <c>gender</c>, <c>birthdate</c>, <c>zoneinfo</c>, <c>locale</c>, and <c>updated_at</c>.</summary>
        public const string Profile = "profile";
        /// <summary>OPTIONAL. This scope value requests access to the <c>email</c> and <c>email_verified</c> Claims.</summary>
        public const string Email = "email";
        /// <summary>OPTIONAL. This scope value requests access to the <c>address</c> Claim.</summary>
        public const string Address = "address";
        /// <summary>OPTIONAL. This scope value requests access to the <c>phone_number</c> and <c>phone_number_verified</c> Claims.</summary>
        public const string Phone = "phone";
        /// <summary>This scope value MUST NOT be used with the OpenID Connect Implicit Client Implementer's Guide 1.0. See the OpenID Connect Basic Client Implementer's Guide 1.0 (http://openid.net/specs/openid-connect-implicit-1_0.html#OpenID.Basic) for its usage in that subset of OpenID Connect.</summary>
        public const string OfflineAccess = "offline_access";
    }
    
    public static class HttpHeaders
    {
        public const string DPoP = "DPoP";
        public const string DPoPNonce = "DPoP-Nonce";
    }
}