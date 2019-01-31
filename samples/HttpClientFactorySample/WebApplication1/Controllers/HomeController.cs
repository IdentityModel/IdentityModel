using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<string> UseNativeHttpClient()
        {
            using (var client = new HttpClient())
            {
                var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = "https://demo.identityserver.io/connect/token",
                    ClientId = "client",
                    ClientSecret = "secret"
                });

                return response.AccessToken ?? response.Error;
            }
        }

        public async Task<string> UseHttpClientFactory([FromServices] IHttpClientFactory httpClientFactory)
        {
            using (var client = httpClientFactory.CreateClient())
            {
                var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = "https://demo.identityserver.io/connect/token",
                    ClientId = "client",
                    ClientSecret = "secret"
                });

                return response.AccessToken ?? response.Error;
            }
        }

        public async Task<string> UseHttpClientFactoryWithName([FromServices] IHttpClientFactory httpClientFactory)
        {
            using (var client = httpClientFactory.CreateClient("token_client"))
            {
                var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    ClientId = "client",
                    ClientSecret = "secret"
                });

                return response.AccessToken ?? response.Error;
            }
        }

        public async Task<string> UseTypedClient([FromServices] TokenClient tokenClient)
        {
            return await tokenClient.GetToken();
        }
    }
}
