using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class TokenClient
    {
        public TokenClient(HttpClient client, TokenClientOptions options)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Options = options ?? new TokenClientOptions();
        }

        public HttpClient Client { get; }

        private readonly TokenClientOptions Options;

        public void ApplyRequestParameters(TokenRequest request, IDictionary<string, string> parameters)
        {
            request.Address = Options.Address;
            request.ClientId = Options.ClientId;
            request.ClientSecret = Options.ClientSecret;
            request.ClientAssertion = Options.ClientAssertion;
            request.ClientCredentialStyle = Options.ClientCredentialStyle;
            request.AuthorizationHeaderStyle = Options.AuthorizationHeaderStyle;

            if (parameters != null)
            {
                request.Parameters = parameters;
            }
        }

        public Task<TokenResponse> RequestClientCredentialsTokenAsync(string scope = null, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
        {
            var request = new ClientCredentialsTokenRequest
            {
                Scope = scope
            };
            ApplyRequestParameters(request, parameters);

            return Client.RequestClientCredentialsTokenAsync(request);
        }

        public Task<TokenResponse> RequestDeviceTokenAsync(string deviceCode, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
        {
            var request = new DeviceTokenRequest
            {
                DeviceCode = deviceCode
            };
            ApplyRequestParameters(request, parameters);

            return Client.RequestDeviceTokenAsync(request);
        }

        public Task<TokenResponse> RequestPasswordTokenAsync(string userName, string password = null, string scope = null, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
        {
            var request = new PasswordTokenRequest
            {
                UserName = userName, 
                Password = password,
                Scope = scope
            };
            ApplyRequestParameters(request, parameters);

            return Client.RequestPasswordTokenAsync(request);
        }

        public Task<TokenResponse> RequestAuthorizationCodeTokenAsync(string code, string redirectUri, string codeVerifier = null, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
        {
            var request = new AuthorizationCodeTokenRequest
            {
                Code = code,
                RedirectUri = redirectUri,
                CodeVerifier = codeVerifier
            };
            ApplyRequestParameters(request, parameters);

            return Client.RequestAuthorizationCodeTokenAsync(request, cancellationToken);
        }

        public Task<TokenResponse> RequestTokenAsync(string grantType, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
        {
            var request = new TokenRequest
            {
                GrantType = grantType
            };
            ApplyRequestParameters(request, parameters);

            return Client.RequestTokenAsync(request, cancellationToken);
        }

        public Task<TokenResponse> RequestRefreshTokenAsync(string refreshToken, string scope = null, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
        {
            var request = new RefreshTokenRequest
            {
                RefreshToken = refreshToken,
                Scope = scope
            };
            ApplyRequestParameters(request, parameters);

            return Client.RequestRefreshTokenAsync(request, cancellationToken);
        }
    }
}