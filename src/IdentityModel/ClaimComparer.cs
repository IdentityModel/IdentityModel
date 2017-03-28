// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityModel
{
    /// <summary>
    /// Claim equality comparer
    /// </summary>
    public class ClaimComparer : IEqualityComparer<Claim>
    {
        private readonly bool _valueAndTypeOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimComparer"/> class.
        /// </summary>
        public ClaimComparer()
        {
            _valueAndTypeOnly = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimComparer"/> class.
        /// </summary>
        /// <param name="compareValueAndTypeOnly">if set to <c>true</c> only type and value are being compared.</param>
        public ClaimComparer(bool compareValueAndTypeOnly)
        {
            _valueAndTypeOnly = compareValueAndTypeOnly;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(Claim x, Claim y)
        {
            if (x == null && y == null) return true;
            if (x == null && y != null) return false;
            if (x != null && y == null) return false;

            if (_valueAndTypeOnly)
            {
                return (x.Type == y.Type &&
                        x.Value == y.Value);
            }
            else
            {
                return (x.Type == y.Type &&
                        x.Value == y.Value &&
                        x.Issuer == y.Issuer &&
                        x.ValueType == y.ValueType);
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(Claim claim)
        {
            if (Object.ReferenceEquals(claim, null)) return 0;

            int typeHash = claim.Type?.GetHashCode() ?? 0;
            int valueHash = claim.Value?.GetHashCode() ?? 0;

            return typeHash ^ valueHash;
        }
    }
}