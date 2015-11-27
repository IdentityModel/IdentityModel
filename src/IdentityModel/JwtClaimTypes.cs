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

namespace IdentityModel
{
    public static class JwtClaimTypes
    {
        // core oidc claims
        public const string Subject             = "sub";
        public const string Name                = "name";
        public const string GivenName           = "given_name";
        public const string FamilyName          = "family_name";
        public const string MiddleName          = "middle_name";
        public const string NickName            = "nickname";
        public const string PreferredUserName   = "preferred_username";
        public const string Profile             = "profile";
        public const string Picture             = "picture";
        public const string WebSite             = "website";
        public const string Email               = "email";
        public const string EmailVerified       = "email_verified";
        public const string Gender              = "gender";
        public const string BirthDate           = "birthdate";
        public const string ZoneInfo            = "zoneinfo";
        public const string Locale              = "locale";
        public const string PhoneNumber         = "phone_number";
        public const string PhoneNumberVerified = "phone_number_verified";
        public const string Address             = "address";
        public const string Audience            = "aud";
        public const string Issuer              = "iss";
        public const string NotBefore           = "nbf";
        public const string Expiration          = "exp";

        // more standard claims
        public const string UpdatedAt                           = "updated_at";
        public const string IssuedAt                            = "iat";
        public const string AuthenticationMethod                = "amr";
        public const string AuthenticationContextClassReference = "acr";
        public const string AuthenticationTime                  = "auth_time";
        public const string AuthorizedParty                     = "azp";
        public const string AccessTokenHash                     = "at_hash";
        public const string AuthorizationCodeHash               = "c_hash";
        public const string Nonce                               = "nonce";

        // more claims
        public const string ClientId         = "client_id";
        public const string Scope            = "scope";
        public const string Id               = "id";
        public const string Secret           = "secret";
        public const string IdentityProvider = "idp";
        public const string Role             = "role";
    }
}