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
    /// <summary>
    /// Client for retrieving OpenID Connect discovery documents
    /// </summary>
    [Obsolete("This type will be deprecated or changed in a future version. It is recommended that you switch to the new extension methods for HttpClient. They give you much more control over the HttpClient lifetime and configuration. See the docs here: https://identitymodel.readthedocs.io")]
    public class DiscoveryClient : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Retrieves a discovery document using the default policy.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <returns></returns>
        public static async Task<DiscoveryResponse> GetAsync(string authority)
        {
            using (var client = new DiscoveryClient(authority))
            {
                return await client.GetAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Parses a URL and turns it into authority and discovery endpoint URL.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// Malformed URL
        /// </exception>
        public static DiscoveryEndpoint ParseUrl(string input)
        {
            var success = Uri.TryCreate(input, UriKind.Absolute, out var uri);
            if (success == false)
            {
                throw new InvalidOperationException("Malformed URL");
            }

            if (!DiscoveryEndpoint.IsValidScheme(uri))
            {
                throw new InvalidOperationException("Malformed URL");
            }

            var url = input.RemoveTrailingSlash();

            if (url.EndsWith(OidcConstants.Discovery.DiscoveryEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                return new DiscoveryEndpoint(url.Substring(0, url.Length - OidcConstants.Discovery.DiscoveryEndpoint.Length - 1), url);
            }
            else
            {
                return new DiscoveryEndpoint(url, url.EnsureTrailingSlash() + OidcConstants.Discovery.DiscoveryEndpoint);
            }
        }

        /// <summary>
        /// The HTTP client
        /// </summary>
        protected readonly HttpClient Client;

        /// <summary>
        /// Gets the authority.
        /// </summary>
        /// <value>
        /// The authority.
        /// </value>
        public string Authority { get; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; }

        /// <summary>
        /// Gets or sets the policy.
        /// </summary>
        /// <value>
        /// The policy.
        /// </value>
        public DiscoveryPolicy Policy { get; set; } = new DiscoveryPolicy();

        /// <summary>
        /// Sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public TimeSpan Timeout
        {
            set
            {
                Client.Timeout = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryClient"/> class.
        /// </summary>
        /// <param name="authority">The authority.</param>
        /// <param name="innerHandler">The inner handler.</param>
        public DiscoveryClient(string authority, HttpMessageHandler innerHandler = null)
        {
            var handler = innerHandler ?? new HttpClientHandler();

            var parsed = ParseUrl(authority);
            Authority = parsed.Authority;
            Url = parsed.Url;

            Client = new HttpClient(handler);
        }

        /// <summary>
        /// Retrieves the discovery document.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<DiscoveryResponse> GetAsync(CancellationToken cancellationToken = default)
        {
            if (Policy.Authority.IsMissing())
            {
                Policy.Authority = Authority;
            }

            string jwkUrl = "";

            if (!DiscoveryEndpoint.IsSecureScheme(new Uri(Url), Policy))
            {
                return new DiscoveryResponse(new InvalidOperationException("HTTPS required"), $"Error connecting to {Url}");
            }

            try
            {
                var response = await Client.GetAsync(Url, cancellationToken).ConfigureAwait(false);
                string responseContent = null;

                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new DiscoveryResponse(response.StatusCode, $"Error connecting to {Url}: {response.ReasonPhrase}", responseContent);
                }

                var disco = new DiscoveryResponse(responseContent, Policy);
                if (disco.IsError)
                {
                    return disco;
                }

                try
                {
                    jwkUrl = disco.JwksUri;
                    if (jwkUrl != null)
                    {
                        response = await Client.GetAsync(jwkUrl, cancellationToken).ConfigureAwait(false);
                        
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
                return new DiscoveryResponse(ex, $"Error connecting to {Url}");
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                Client.Dispose();
            }
        }
    }
}