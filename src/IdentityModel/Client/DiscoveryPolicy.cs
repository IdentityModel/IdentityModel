// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace IdentityModel.Client
{
    /// <summary>
    /// Security policy for retrieving a discovery document
    /// </summary>
    public class DiscoveryPolicy
    {
        internal string Authority;

        /// <summary>
        /// Specifies if HTTPS is enforced on all endpoints. Defaults to true.
        /// </summary>
        public bool RequireHttps { get; set; } = true;

        /// <summary>
        /// Specifies if HTTP is allowed on loopback addresses. Defaults to true.
        /// </summary>
        public bool AllowHttpOnLoopback { get; set; } = true;

        /// <summary>
        /// Specifies valid loopback addresses, defaults to localhost and 127.0.0.1
        /// </summary>
        public ICollection<string> LoopbackAddresses = new HashSet<string> { "localhost", "127.0.0.1" };

        /// <summary>
        /// Specifies if the issuer name is checked to be identical to the authority. Defaults to true.
        /// </summary>
        public bool ValidateIssuerName { get; set; } = true;

        /// <summary>
        /// Specifies if all endpoints are checked to belong to the authority. Defaults to true.
        /// </summary>
        public bool ValidateEndpoints { get; set; } = true;

        /// <summary>
        /// Specifies a list of endpoints that should be excluded from validation
        /// </summary>
        public ICollection<string> EndpointValidationExcludeList { get; set; } = new HashSet<string>();

        /// <summary>
        /// Specifies a list of additional base addresses that should be allowed for endpoints
        /// </summary>
        public ICollection<string> AdditionalEndpointBaseAddresses { get; set; } = new HashSet<string>();

        /// <summary>
        /// Specifies if a key set is required. Defaults to true.
        /// </summary>
        public bool RequireKeySet { get; set; } = true;
    }
}