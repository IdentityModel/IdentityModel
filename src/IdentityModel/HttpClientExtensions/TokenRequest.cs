using IdentityModel.Client;
using System.Collections.Generic;

namespace IdentityModel.HttpClientExtensions
{
    public class TokenRequest
    {
        public string Address { get; set; }
        
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public CredentialStyle CredentialStyle { get; set; } = CredentialStyle.PostBody;

        public string Scope { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
    }

    public class PasswordTokenRequest : TokenRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
