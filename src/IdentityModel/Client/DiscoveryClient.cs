using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class DiscoveryClient
    {
        private readonly HttpClient _client;

        public string Url { get; }

        public TimeSpan Timeout
        {
            set
            {
                _client.Timeout = value;
            }
        }

        public DiscoveryClient(string url, HttpMessageHandler innerHandler = null)
        {
            var handler = innerHandler ?? new HttpClientHandler();

            url = url.RemoveTrailingSlash();
            if (!url.EndsWith(OidcConstants.Discovery.DiscoveryEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                url = url.EnsureTrailingSlash();
                url = url + OidcConstants.Discovery.DiscoveryEndpoint;
            }

            Url = url;
            
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