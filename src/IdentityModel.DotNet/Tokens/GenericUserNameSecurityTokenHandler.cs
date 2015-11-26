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

#if NET451

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityModel.Tokens
{
    /// <summary>
    /// Generic security token handler for username/password type credentials
    /// </summary>
    public class GenericUserNameSecurityTokenHandler : UserNameSecurityTokenHandler
    {
        /// <summary>
        /// Callback type for validating the credential
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True when the credential could be validated succesfully. Otherwise false.</returns>
        public delegate bool ValidateUserNameCredentialDelegate(string username, string password);

        /// <summary>
        /// Gets or sets the credential validation callback
        /// </summary>
        /// <value>
        /// The credential validation callback.
        /// </value>
        public ValidateUserNameCredentialDelegate ValidateUserNameCredential { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUserNameSecurityTokenHandler"/> class.
        /// </summary>
        public GenericUserNameSecurityTokenHandler()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUserNameSecurityTokenHandler"/> class.
        /// </summary>
        /// <param name="validateUserNameCredential">The credential validation callback.</param>
        public GenericUserNameSecurityTokenHandler(ValidateUserNameCredentialDelegate validateUserNameCredential)
        {
            if (validateUserNameCredential == null)
            {
                throw new ArgumentNullException("ValidateUserNameCredential");
            }

            ValidateUserNameCredential = validateUserNameCredential;
        }

        /// <summary>
        /// Validates the user name credential core.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected virtual bool ValidateUserNameCredentialCore(string userName, string password)
        {
            if (ValidateUserNameCredential == null)
            {
                throw new InvalidOperationException("ValidateUserNameCredentialDelegate not set");
            }

            return ValidateUserNameCredential(userName, password);
        }

        /// <summary>
        /// Validates the username and password.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A ClaimsIdentityCollection representing the identity in the token</returns>
        public override ReadOnlyCollection<ClaimsIdentity> ValidateToken(SecurityToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            if (Configuration == null)
            {
                throw new InvalidOperationException("No Configuration set");
            }

            UserNameSecurityToken unToken = token as UserNameSecurityToken;
            if (unToken == null)
            {
                throw new ArgumentException("SecurityToken is not a UserNameSecurityToken");
            }

            if (!ValidateUserNameCredentialCore(unToken.UserName, unToken.Password))
            {
                throw new SecurityTokenValidationException(unToken.UserName);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, unToken.UserName),
                new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.Password),
                AuthenticationInstantClaim.Now
            };

            var identity = new ClaimsIdentity(claims);

            if (Configuration.SaveBootstrapContext)
            {
                if (RetainPassword)
                {
                    identity.BootstrapContext = new BootstrapContext(unToken, this);
                }
                else
                {
                    identity.BootstrapContext = new BootstrapContext(new UserNameSecurityToken(unToken.UserName, null), this);
                }
            }

            return new List<ClaimsIdentity> { new ClaimsIdentity(claims, "Password") }.AsReadOnly();
        }

        /// <summary>
        /// Gets a value indicating whether this instance can validate a token.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can validate a token; otherwise, <c>false</c>.
        /// </value>
        public override bool CanValidateToken
        {
            get
            {
                return true;
            }
        }
    }
}

#endif