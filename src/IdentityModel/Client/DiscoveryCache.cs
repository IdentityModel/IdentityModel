// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.HttpClientExtensions;
using IdentityModel.Internal;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Helper for caching discovery documents.
    /// </summary>
    public class DiscoveryCache
    {
        private DateTime _nextReload = DateTime.MinValue;
        private AsyncLazy<DiscoveryResponse> _lazyResponse;

        private readonly DiscoveryPolicy _policy;
        private readonly HttpClient _client;
        private readonly string _authority;

        /// <summary>
        /// Initialize instance of DiscoveryCache with passed authority.
        /// </summary>
        /// <param name="authority">Base address or discovery document endpoint.</param>
        /// <param name="client">The client.</param>
        /// <param name="policy">The policy.</param>
        public DiscoveryCache(string authority, HttpClient client = default, DiscoveryPolicy policy = default)
        {
            _authority = authority;
            _client = client;
            _policy = policy;
        }

        /// <summary>
        /// Initialize instance of DiscoveryCache with passed DiscoveryClient.
        /// </summary>
        /// <param name="client">DiscoveryClient to use for obtaining discovery document.</param>
        public DiscoveryCache(DiscoveryClient client)
        {
            _client = new HttpClient();
            _policy = client.Policy;
            _authority = client.Authority;
        }

        /// <summary>
        /// Frequency to refresh discovery document. Defaults to 24 hours.
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// Get the DiscoveryResponse either from cache or from discovery endpoint.
        /// </summary>
        /// <returns></returns>
        public Task<DiscoveryResponse> GetAsync()
        {
            if (_nextReload <= DateTime.UtcNow)
            {
                Refresh();
            }

            return _lazyResponse.Value;
        }

        /// <summary>
        /// Marks the discovery document as stale and will trigger a request to the discovery endpoint on the next request to get the DiscoveryResponse.
        /// </summary>
        public void Refresh()
        {
            _lazyResponse = new AsyncLazy<DiscoveryResponse>(GetResponseAsync);
        }

        private async Task<DiscoveryResponse> GetResponseAsync()
        {
            var result = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _authority,
                Policy = _policy
            });

            _nextReload = DateTime.UtcNow.Add(CacheDuration);
            return result;
        }
    }
}
