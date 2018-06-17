using IdentityModel.Client;
using System.Collections.Generic;

namespace IdentityModel.HttpClientExtensions
{
    public class TokenRevocationRequest
    {
        public string Address { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;
        public BasicAuthenticationHeaderStyle BasicAuthenticationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

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

        /// <summary>
        /// Gets or sets additional parameters on the request.
        /// </summary>
        /// <value>
        /// Additional parameters.
        /// </value>
        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
