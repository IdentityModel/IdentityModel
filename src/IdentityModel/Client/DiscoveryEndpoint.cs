using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    /// <summary>
    /// Represents a URL to a discovery endpoint - parsed to separate the URL and authority
    /// </summary>
    public class DiscoveryEndpoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Url"/> class.
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
}
