using IdentityModel.Internal;
using System;

namespace IdentityModel.Client;

/// <summary>
/// Represents a URL to a discovery endpoint - parsed to separate the URL and authority
/// </summary>
public class DiscoveryEndpoint
{
    /// <summary>
    /// Parses a URL and turns it into authority and discovery endpoint URL.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="path">The path to the discovery document. If not specified this defaults to .well-known/open-id-configuration</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">
    /// Malformed URL
    /// </exception>
    public static DiscoveryEndpoint ParseUrl(string input, string? path = null)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        
        if (String.IsNullOrEmpty(path))
        {
            path = OidcConstants.Discovery.DiscoveryEndpoint;
        }

        var success = Uri.TryCreate(input, UriKind.Absolute, out var uri);
        if (success == false)
        {
            throw new InvalidOperationException("Malformed URL");
        }

        if (!DiscoveryEndpoint.IsValidScheme(uri!))
        {
            throw new InvalidOperationException("Malformed URL");
        }

        var url = input.RemoveTrailingSlash();
        if (path!.StartsWith("/"))
        {
            path = path.Substring(1);
        }

        if (url.EndsWith(path, StringComparison.OrdinalIgnoreCase))
        {
            return new DiscoveryEndpoint(url.Substring(0, url.Length - path.Length - 1), url);
        }
        else
        {
            return new DiscoveryEndpoint(url, url.EnsureTrailingSlash() + path);
        }
    }

    /// <summary>
    /// Determines whether the URL uses http or https.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>
    ///   <c>true</c> if [is valid scheme] [the specified URL]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidScheme(Uri url)
    {
        if (string.Equals(url.Scheme, "http", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(url.Scheme, "https", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether uses a secure scheme accoding to the policy.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="policy">The policy.</param>
    /// <returns>
    ///   <c>true</c> if [is secure scheme] [the specified URL]; otherwise, <c>false</c>.
    /// </returns>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoveryEndpoint"/> class.
    /// </summary>
    /// <param name="authority">The authority.</param>
    /// <param name="url">The discovery endpoint URL.</param>
    public DiscoveryEndpoint(string authority, string url)
    {
        Authority = authority;
        Url = url;
    }
    /// <summary>
    /// Gets or sets the authority.
    /// </summary>
    /// <value>
    /// The authority.
    /// </value>
    public string Authority { get; }

    /// <summary>
    /// Gets or sets the discovery endpoint.
    /// </summary>
    /// <value>
    /// The discovery endpoint.
    /// </value>
    public string Url { get; }
}