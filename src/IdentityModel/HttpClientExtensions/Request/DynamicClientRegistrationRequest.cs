using IdentityModel.Client;
using System.Collections.Generic;

namespace IdentityModel.HttpClientExtensions
{
    public class DynamicClientRegistrationRequest
    {
        public string Address { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;
        public BasicAuthenticationHeaderStyle BasicAuthenticationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();


        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        public RegistrationRequest RegistrationRequest  { get; set; }
    }
}
