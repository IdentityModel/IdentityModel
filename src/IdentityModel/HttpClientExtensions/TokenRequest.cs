using IdentityModel.Client;
using System.Collections.Generic;

namespace IdentityModel.HttpClientExtensions
{
    public class TokenRequest
    {
        public string Address { get; set; }
        public string GrantType { get; set; }
        
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ClientCredentialStyle CredentialStyle { get; set; } = ClientCredentialStyle.PostBody;

        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }

    public class ClientCredentialsTokenRequest : TokenRequest
    {
        public string Scope { get; set; }
    }

    public class PasswordTokenRequest : TokenRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Scope { get; set; }
    }

    public class AuthorizationCodeTokenRequest : TokenRequest
    {
        public string Code { get; set; }
        public string RedirectUri { get; set; }

        public string CodeVerifier { get; set; }
    }

    public class RefreshTokenRequest : TokenRequest
    {
        public string RefreshToken { get; set; }

        public string Scope { get; set; }
    }
}