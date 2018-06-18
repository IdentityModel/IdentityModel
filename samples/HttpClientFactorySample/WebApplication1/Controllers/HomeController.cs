using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using IdentityModel.HttpClientExtensions;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public IHttpClientFactory HttpClientFactory { get; }
        public TokenClient TokenClient { get; }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<string> NoFactory()
        {
            var client = new HttpClient();

            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://demo.identityserver.io/connect/token",
                ClientId = "client",
                ClientSecret = "secret"
            });

            return response.AccessToken ?? response.Error;
        }

        public async Task<string> Simple()
        {
            var client = HttpClientFactory.CreateClient();

            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://demo.identityserver.io/connect/token",
                ClientId = "client",
                ClientSecret = "secret"
            });

            return response.AccessToken ?? response.Error;
        }

        public async Task<string> WithAddress()
        {
            var client = HttpClientFactory.CreateClient("token_client");

            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                ClientId = "client",
                ClientSecret = "secret"
            });

            return response.AccessToken ?? response.Error;
        }

        public async Task<string> Typed([FromServices] TokenClient tokenClient)
        {
            return await tokenClient.GetToken();
        }
    }
}
