// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Internal;
using IdentityModel.Jwk;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public class DiscoveryClient
    {
        public static async Task<DiscoveryResponse> GetAsync(string authority)
        {
            var client = new DiscoveryClient(authority);
            return await client.GetAsync().ConfigureAwait(false);
        }

        private readonly HttpClient _client;

        public string Authority { get; }
        public string Url { get; }

        public DiscoveryPolicy Policy { get; set; } = new DiscoveryPolicy();

        public TimeSpan Timeout
        {
            set
            {
                _client.Timeout = value;
            }
        }

        public DiscoveryClient(string authority, HttpMessageHandler innerHandler = null)
        {
            var handler = innerHandler ?? new HttpClientHandler();

            Uri uri;
            var success = Uri.TryCreate(authority, UriKind.Absolute, out uri);
            if (success == false)
            {
                throw new InvalidOperationException("Malformed authority URL");
            }

            if (!DiscoveryUrlHelper.IsValidScheme(uri, Policy))
            {
                throw new InvalidOperationException("Malformed authority URL");
            }

            var url = authority.RemoveTrailingSlash();
            if (url.EndsWith(OidcConstants.Discovery.DiscoveryEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                Url = url;
                Authority = url.Substring(0, url.Length - OidcConstants.Discovery.DiscoveryEndpoint.Length - 1);
            }
            else
            {
                Authority = url;
                Url = url.EnsureTrailingSlash() + OidcConstants.Discovery.DiscoveryEndpoint;
            }

            _client = new HttpClient(handler);
        }

        public async Task<DiscoveryResponse> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Policy.Authority = Authority;

            if (!DiscoveryUrlHelper.IsSecureScheme(new Uri(Url), Policy))
            {
                throw new InvalidOperationException($"Policy demands the usage of HTTPS: {Url}");
            }

            try
            {
                var response = await _client.GetAsync(Url, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    return new DiscoveryResponse(response.StatusCode, response.ReasonPhrase);
                }

                var disco = new DiscoveryResponse(await response.Content.ReadAsStringAsync().ConfigureAwait(false), Policy);
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