// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityModel
{
    /// <summary>
    /// Compares two instances of Claim
    /// </summary>
    public class ClaimComparer : EqualityComparer<Claim>
    {
        /// <summary>
        /// Claim comparison options
        /// </summary>
        public class Options
        {
            /// <summary>
            /// Specifies if the issuer value is being taken into account
            /// </summary>
            public bool IgnoreIssuer { get; set; } = false;

            /// <summary>
            /// Specifies if value comparison should be case-sensitive
            /// </summary>
            public bool IgnoreCase { get; set; } = true;
        }

        private readonly Options _options = new Options();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimComparer"/> class with default options.
        /// </summary>
        public ClaimComparer()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimComparer"/> class with given comparison options.
        /// </summary>
        /// <param name="options">Comparison options.</param>
        public ClaimComparer(Options options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        public override bool Equals(Claim x, Claim y)
        {
            if (x == null && y == null) return true;
            if (x == null && y != null) return false;
            if (x != null && y == null) return false;

            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (_options.IgnoreCase == false) comparison = StringComparison.Ordinal;
            
            if (_options.IgnoreIssuer)
            {
                return (String.Equals(x.Type, y.Type, comparison) &&
                        String.Equals(x.Value, y.Value, comparison));
            }
            else
            {
                return (String.Equals(x.Type, y.Type, comparison) &&
                        String.Equals(x.Value, y.Value, comparison) &&
                        String.Equals(x.Issuer, y.Issuer, comparison));
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode(Claim claim)
        {
            if (claim is null) return 0;

            int typeHash;
            int valueHash;
            int issuerHash;

            if (_options.IgnoreCase)
            {
                typeHash = claim.Type?.ToLowerInvariant().GetHashCode() ?? 0;
                valueHash = claim.Value?.ToLowerInvariant().GetHashCode() ?? 0;
                issuerHash = claim.Issuer?.ToLowerInvariant().GetHashCode() ?? 0;
            }
            else
            {
                typeHash = claim.Type?.GetHashCode() ?? 0;
                valueHash = claim.Value?.GetHashCode() ?? 0;
                issuerHash = claim.Issuer?.GetHashCode() ?? 0;
            }

            if (_options.IgnoreIssuer)
            {
                return typeHash ^ valueHash;
                
            }
            else
            {
                return typeHash ^ valueHash ^ issuerHash;
            }
        }
    }
}