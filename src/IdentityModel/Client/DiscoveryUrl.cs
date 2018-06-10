// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using IdentityModel.Internal;
using System;

namespace IdentityModel.Client
{
    public static class DiscoveryUrl
    {
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

            if (!DiscoveryUrl.IsValidScheme(uri))
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

        public static bool IsValidScheme(Uri url)
        {
            if (string.Equals(url.Scheme, "http", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool IsSecureScheme(Uri url, DiscoveryPolicy policy)
        {
            if (policy.RequireHttps == true)
            {
                if (policy.AllowHttpOnLoopback == true)
                {
                    var hostName = url.DnsSafeHost;

                    foreach (var address in policy.LoopbackAddresses)
                    {
                        if (string.Equals(hostName, address, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }

                return string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }
    }
}