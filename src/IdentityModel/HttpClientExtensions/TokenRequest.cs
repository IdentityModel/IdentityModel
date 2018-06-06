using IdentityModel.Client;

namespace IdentityModel.HttpClientExtensions
{
    public class TokenRequest
    {
        public string Address { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public CredentialStyle CredentialStyle { get; set; } = CredentialStyle.PostBody;

        public string Scope { get; set; }

        public object Parameters { get; set; }
    }
}
