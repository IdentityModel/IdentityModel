using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class DemoIdentityServerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public Task<string> UseTypedClient([FromServices] DemoIdentityServerClient demoClient, CancellationToken cancellationToken)
        {
            return demoClient.GetTest(cancellationToken);
        }
    }
}