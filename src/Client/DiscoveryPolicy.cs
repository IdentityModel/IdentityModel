// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace IdentityModel.Client
{
    /// <summary>
    /// Security policy for retrieving a discovery document
    /// </summary>
    public class DiscoveryPolicy
    {
        private static readonly StringComparison defaultAuthorityNameComparison = StringComparison.Ordinal;
        private StringComparison _authorityNameComparison = defaultAuthorityNameComparison;

        /// <summary>
        /// Gets or sets the Authority on which the policy checks will be based on
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Method of comparison for issuer and authority names. Defaults to <see cref="StringComparison.Ordinal" />
        /// </summary>
        [Obsolete("Please use AuthorityValidationStrategy")]
        public StringComparison AuthorityNameComparison
        {
            get
            {
                return _authorityNameComparison;
            }
            set
            {
                _authorityNameComparison = value;
                AuthorityValidationStrategy = new StringComparisonAuthorityValidationStrategy(value);
            }
        }

        /// <summary>
        /// Strategy used to validate issuer name and endpoints based on expected authority.
        /// Defaults to <see cref="StringComparisonAuthorityValidationStrategy.Default"/>.
        /// </summary>
        public IAuthorityValidationStrategy AuthorityValidationStrategy { get; set; } = new StringComparisonAuthorityValidationStrategy(defaultAuthorityNameComparison);

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