// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityModel.Client
{
    /// <summary>
    /// Request for OpenID Connect discovery document
    /// </summary>
    public class DiscoveryDocumentRequest
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the policy.
        /// </summary>
        /// <value>
        /// The policy.
        /// </value>
        public DiscoveryPolicy Policy { get; set; } = new DiscoveryPolicy();
    }
}