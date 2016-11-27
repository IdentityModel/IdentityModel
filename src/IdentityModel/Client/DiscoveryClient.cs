// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Jwk;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class DiscoveryClient
    {
        public static async Task<DiscoveryResponse> GetAsync(string url)
        {
            var client = new DiscoveryClient(url);
            return await client.GetAsync().ConfigureAwait(false);
        }

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
            try
            {
                var response = await _client.GetAsync(Url, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    return new DiscoveryResponse(response.StatusCode, response.ReasonPhrase);
                }

                var disco = new DiscoveryResponse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                if (disco.IsError)
                {
                    return disco;
                }

                var jwkUrl = disco.JwksUri;
                if (jwkUrl != null)
                {
                    response = await _client.GetAsync(jwkUrl, cancellationToken).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        return new DiscoveryResponse(response.StatusCode, response.ReasonPhrase);
                    }

                    var jwk = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    disco.KeySet = new JsonWebKeySet(jwk);
                }

                return disco;
            }
            catch (Exception ex)
            {
                return new DiscoveryResponse(ex);
            }
        }
    }
}