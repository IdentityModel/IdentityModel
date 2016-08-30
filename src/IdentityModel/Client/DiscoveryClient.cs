using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class DiscoveryClient
    {
        private readonly HttpClient _client;

        public TimeSpan Timeout
        {
            set
            {
                _client.Timeout = value;
            }
        }

        public DiscoveryClient(string baseAddress, HttpMessageHandler innerHandler = null)
        {
            var handler = innerHandler ?? new HttpClientHandler();

            // todo: check slashes
            var url = baseAddress + OidcConstants.Discovery.DiscoveryEndpoint;

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(url)
            };
        }

        public async Task<DiscoveryResponse> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _client.GetAsync("", cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new DiscoveryResponse(json);
            }
            else
            {
                return new DiscoveryResponse(response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}