using IdentityModel.Client;
using IdentityModel.Internal;
using IdentityModel.Jwk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.HttpClientExtensions
{
    public static class HttpClientDiscoveryExtensions
    {
        public static async Task<DiscoveryResponse> GetAsync(this HttpClient client, string address, DiscoveryPolicy policy = null, CancellationToken cancellationToken = default)
        {
            if (policy == null) policy = new DiscoveryPolicy();
            if (address.IsMissing()) address = client.BaseAddress.AbsoluteUri;

            var parsed = DiscoveryClient.ParseUrl(address);
            var authority = parsed.Authority;
            var url = parsed.Url;

            if (policy.Authority.IsMissing())
            {
                policy.Authority = authority;
            }

            string jwkUrl = "";

            if (!DiscoveryUrl.IsSecureScheme(new Uri(url), policy))
            {
                return new DiscoveryResponse(new InvalidOperationException("HTTPS required"), $"Error connecting to {url}");
            }

            try
            {
                var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
                string responseContent = null;

                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new DiscoveryResponse(response.StatusCode, $"Error connecting to {url}: {response.ReasonPhrase}", responseContent);
                }

                var disco = new DiscoveryResponse(responseContent, policy);
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