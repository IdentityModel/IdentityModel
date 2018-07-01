using IdentityModel.Client;
using System.Collections.Generic;

namespace IdentityModel.HttpClientExtensions
{
    public class TokenIntrospectionRequest : Request
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the token type hint.
        /// </summary>
        /// <value>
        /// The token type hint.
        /// </value>
        public string TokenTypeHint { get; set; }
    }
}