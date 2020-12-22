// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class ClaimComparisonTests
    {
        private readonly List<Claim> _claims = new List<Claim>
        {
            new Claim("claim_type1", "value", ClaimValueTypes.String, "issuer1"),
            new Claim("claim_type1", "value", ClaimValueTypes.String, "issuer1"),
            new Claim("claim_type1", "Value", ClaimValueTypes.String, "issuer1"),

            new Claim("Claim_type1", "value", ClaimValueTypes.String, "issuer1"),
            new Claim("Claim_type1", "value", ClaimValueTypes.String, "issuer1"),
            new Claim("Claim_type1", "Value", ClaimValueTypes.String, "issuer1"),

            new Claim("claim_type1", "value", ClaimValueTypes.String, "issuer2"),
            new Claim("claim_type1", "value", ClaimValueTypes.String, "issuer2"),
            new Claim("claim_type1", "Value", ClaimValueTypes.String, "issuer2"),

            new Claim("Claim_type1", "value", ClaimValueTypes.String, "issuer2"),
            new Claim("Claim_type1", "value", ClaimValueTypes.String, "issuer2"),
            new Claim("Claim_type1", "Value", ClaimValueTypes.String, "issuer2")
        };


        
        [Fact]
        public void Default_options_should_result_in_four_claims()
        {
            var hashSet = new HashSet<Claim>(_claims, new ClaimComparer());

            hashSet.Count.Should().Be(4);

            var item = hashSet.First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("value");
            item.Issuer.Should().Be("issuer1");

            item = hashSet.Skip(1).First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("Value");
            item.Issuer.Should().Be("issuer1");

            item = hashSet.Skip(2).First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("value");
            item.Issuer.Should().Be("issuer2");

            item = hashSet.Skip(3).First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("Value");
            item.Issuer.Should().Be("issuer2");

        }

        [Fact]
        public void Ordinal_should_result_in_four_claims()
        {
            var hashSet = new HashSet<Claim>(_claims, new ClaimComparer(new ClaimComparer.Options { IgnoreValueCase = false }));

            hashSet.Count.Should().Be(4);

            var item = hashSet.First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("value");
            item.Issuer.Should().Be("issuer1");

            item = hashSet.Skip(1).First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("Value");
            item.Issuer.Should().Be("issuer1");

            item = hashSet.Skip(2).First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("value");
            item.Issuer.Should().Be("issuer2");

            item = hashSet.Skip(3).First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("Value");
            item.Issuer.Should().Be("issuer2");
        }

        [Fact]
        public void Ignoring_issuer_should_result_in_one_claim()
        {
            var hashSet = new HashSet<Claim>(_claims, new ClaimComparer(new ClaimComparer.Options { IgnoreIssuer = true }));

            hashSet.Count.Should().Be(2);

            var item = hashSet.First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("value");
            item.Issuer.Should().Be("issuer1");

            item = hashSet.Skip(1).First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("Value");
            item.Issuer.Should().Be("issuer1");
        }

        [Fact]
        public void Ordinal_and_ignoring_issuer_should_result_in_two_claims()
        {
            var hashSet = new HashSet<Claim>(_claims, new ClaimComparer(new ClaimComparer.Options { IgnoreValueCase = false, IgnoreIssuer = true }));

            hashSet.Count.Should().Be(2);

            var item = hashSet.First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("value");
            item.Issuer.Should().Be("issuer1");

            item = hashSet.Skip(1).First();
            item.Type.Should().Be("claim_type1");
            item.Value.Should().Be("Value");
            item.Issuer.Should().Be("issuer1");
        }
    }
}