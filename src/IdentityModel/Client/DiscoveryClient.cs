using IdentityModel.Jwk;
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

            _client = new HttpClient(handler);
        }

        public async Task<DiscoveryResponse> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _client.GetAsync(Url, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var disco = new DiscoveryResponse(json);

                var jwkUrl = disco.JwksUri;
                if (jwkUrl != null)
                {
                    response = await _client.GetAsync(jwkUrl).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var jwk = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        disco.Keys = new JsonWebKeySet(jwk);
                    }
                    else
                    {
                        return new DiscoveryResponse(response.StatusCode, response.ReasonPhrase);
                    }
                }

                return disco;
            }
            else
            {
                return new DiscoveryResponse(response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}