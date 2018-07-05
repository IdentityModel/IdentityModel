using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace WebApplication1
{
    public class TokenClient
    {
        public TokenClient(HttpClient client, IOptions<TokenClientOptions> options)
        {
            Client = client;
            Options = options.Value;
        }

        public HttpClient Client { get; }
        public TokenClientOptions Options { get; }

        public async Task<string> GetToken()
        {
            var response = await Client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = Options.Address,
                ClientId = Options.ClientId,
                ClientSecret = Options.ClientSecret
            });

            return response.AccessToken ?? response.Error;
        }
    }
}