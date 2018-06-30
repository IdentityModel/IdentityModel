using IdentityModel.Client;
using IdentityModel.Internal;
using IdentityModel.Jwk;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.HttpClientExtensions
{
    public static class HttpClientDiscoveryExtensions
    {
        public static async Task<DiscoveryResponse> GetDiscoveryDocumentAsync(this HttpClient client, string address, CancellationToken cancellationToken = default)
        {
            return await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest { Address = address }, cancellationToken);
        }

        public static async Task<DiscoveryResponse> GetDiscoveryDocumentAsync(this HttpClient client, DiscoveryDocumentRequest request = null, CancellationToken cancellationToken = default)
        {
            if (request == null) request = new DiscoveryDocumentRequest();

            string address;
            if (request.Address.IsPresent())
            {
                address = request.Address;
            }
            else
            {
                address = client.BaseAddress.AbsoluteUri;
            }

            var parsed = DiscoveryClient.ParseUrl(address);
            var authority = parsed.Authority;
            var url = parsed.Url;

            if (request.Policy.Authority.IsMissing())
            {
                request.Policy.Authority = authority;
            }

            string jwkUrl = "";

            if (!DiscoveryEndpoint.IsSecureScheme(new Uri(url), request.Policy))
            {
                return new DiscoveryResponse(new InvalidOperationException("HTTPS required"), $"Error connecting to {url}");
            }

            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

                string responseContent = null;

                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new DiscoveryResponse(response.StatusCode, $"Error connecting to {url}: {response.ReasonPhrase}", responseContent);
                }

                var disco = new DiscoveryResponse(responseContent, request.Policy);
                if (disco.IsError)
                {
                    return disco;
                }

                try
                {
                    jwkUrl = disco.JwksUri;
                    if (jwkUrl != null)
                    {
                        response = await client.GetAsync(jwkUrl, cancellationToken).ConfigureAwait(false);

                        if (response.Content != null)
                        {
                            responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            return new DiscoveryResponse(response.StatusCode, $"Error connecting to {jwkUrl}: {response.ReasonPhrase}", responseContent);
                        }

                        disco.KeySet = new JsonWebKeySet(responseContent);
                    }

                    return disco;
                }
                catch (Exception ex)
                {
                    return new DiscoveryResponse(ex, $"Error connecting to {jwkUrl}");
                }
            }
            catch (Exception ex)
            {
                return new DiscoveryResponse(ex, $"Error connecting to {url}");
            }
        }
    }
}