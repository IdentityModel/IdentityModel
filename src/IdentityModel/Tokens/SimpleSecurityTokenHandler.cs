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
    public class SimpleSecurityTokenHandler : SecurityTokenHandler
    {
        private string[] _identifier;
        public delegate ClaimsPrincipal ValidateTokenDelegate(string tokenString);

        public ValidateTokenDelegate Validator { get; set; }

        public SimpleSecurityTokenHandler(string identifier)
            : this(identifier, null)
        { }

        public SimpleSecurityTokenHandler(ValidateTokenDelegate validator) 
            : this(Guid.NewGuid().ToString(), validator)
        { }

        public SimpleSecurityTokenHandler(string identifier, ValidateTokenDelegate validator)
        {
            _identifier = new string[] { identifier };
            Validator = validator;
        }

        public override SecurityToken ReadToken(string tokenString)
        {
            return new SimpleSecurityToken(tokenString);
        }

        public override ReadOnlyCollection<ClaimsIdentity> ValidateToken(SecurityToken token)
        {
            var simpleToken = token as SimpleSecurityToken;
            if (simpleToken == null)
            {
                throw new ArgumentException("SecurityToken is not a SimpleSecurityToken");
            }

            var identity = Validator(simpleToken.Token).Identity as ClaimsIdentity;

            if (identity != null)
            {
                if (Configuration != null && Configuration.SaveBootstrapContext)
                {
                    identity.BootstrapContext = new BootstrapContext(simpleToken, this);
                }

                return new List<ClaimsIdentity> { identity }.AsReadOnly();
            }
            else
            {
                throw new SecurityTokenValidationException("No identity");
            }
        }

        public override string WriteToken(SecurityToken token)
        {
            var simpleToken = token as SimpleSecurityToken;
            if (simpleToken == null)
            {
                throw new ArgumentException("SecurityToken is not a SimpleSecurityToken");
            }

            return simpleToken.Token;
        }

        public override string[] GetTokenTypeIdentifiers()
        {
            return _identifier;
        }

        public override Type TokenType
        {
            get { return typeof(SimpleSecurityToken); }
        }
    }
}

#endif