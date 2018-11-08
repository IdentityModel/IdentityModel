using IdentityModel.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public static class DiscoveryEndpointHelper
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
    }
}
