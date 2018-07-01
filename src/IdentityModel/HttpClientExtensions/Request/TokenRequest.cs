namespace IdentityModel.HttpClientExtensions
{
    public class TokenRequest : Request
    {
        public string GrantType { get; set; }   
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

    public class AssertionTokenRequest : TokenRequest
    {
        public string AssertionType { get; set; }
        public string Assertion { get; set; }

        public string Scope { get; set; }
    }
}