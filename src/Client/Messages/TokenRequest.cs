// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityModel.Client
{
    /// <summary>
    /// Request for token
    /// </summary>
    /// <seealso cref="Request" />
    public class TokenRequest : Request
    {
        /// <summary>
        /// Gets or sets the type of the grant.
        /// </summary>
        /// <value>
        /// The type of the grant.
        /// </value>
        public string GrantType { get; set; }   
    }

    /// <summary>
    /// Request for token using client_credentials
    /// </summary>
    /// <seealso cref="TokenRequest" />
    public class ClientCredentialsTokenRequest : TokenRequest
    {
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }
    }

    /// <summary>
    /// Request for token using urn:ietf:params:oauth:grant-type:device_code
    /// </summary>
    /// <seealso cref="TokenRequest" />
    public class DeviceTokenRequest : TokenRequest
    {
        /// <summary>
        /// Gets or sets the device code.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string DeviceCode { get; set; }
    }

    /// <summary>
    /// Request for token using password
    /// </summary>
    /// <seealso cref="TokenRequest" />
    public class PasswordTokenRequest : TokenRequest
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }
    }

    /// <summary>
    /// Request for token using authorization_code
    /// </summary>
    /// <seealso cref="TokenRequest" />
    public class AuthorizationCodeTokenRequest : TokenRequest
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>
        /// The redirect URI.
        /// </value>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the code verifier.
        /// </summary>
        /// <value>
        /// The code verifier.
        /// </value>
        public string CodeVerifier { get; set; }
    }

    /// <summary>
    /// Request for token using refresh_token
    /// </summary>
    /// <seealso cref="TokenRequest" />
    public class RefreshTokenRequest : TokenRequest
    {
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }
    }
}