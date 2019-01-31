using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1
{
    public sealed class DemoIdentityServerClient
    {
        public DemoIdentityServerClient(HttpClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public HttpClient Client { get; }

        public async Task<string> GetTest(CancellationToken cancellationToken)
        {
            using (var response = await Client.GetAsync("api/test", cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}