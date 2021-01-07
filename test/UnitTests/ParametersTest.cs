// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class ParameterTest
    {
        [Fact]
        public void Replace_replaces_single_element()
        {
            var p = new Parameters
            {
                { "key1", "value1" }
            };

            p.ReplaceAll("key1", "value2");

            p.GetValues("key1").Single().Should().Be("value2");
        }

        [Fact]
        public void Replace_replaces_multiple_elements()
        {
            var p = new Parameters
            {
                { "key1", "value1" },
                { "key1", "value2" }
            };

            p.ReplaceAll("key1", "value3");

            p.GetValues("key1").Single().Should().Be("value3");
        }

        [Fact]
        public void Get_returns_single_key()
        {
            var p = new Parameters
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            var keys = p.GetValues("key1");
            keys.Should().HaveCount(1);
        }

        [Fact]
        public void Get_returns_multiple_keys()
        {
            var p = new Parameters
            {
                { "key1", "value1" },
                { "key1", "value2" }
            };

            var keys = p.GetValues("key1");
            keys.Should().HaveCount(2);
        }

        [Fact]
        public void AddRequired_adds_correctly()
        {
            var p = new Parameters();

            p.AddRequired("key1", "value");
            p.Should().HaveCount(1);

            p.AddRequired("key2", "value");
            p.Should().HaveCount(2);
            
            p.AddRequired("key3", "value");
            p.Should().HaveCount(3);
        }

        [Fact]
        public void AddRequired_throws_on_empty_key()
        {
            var p = new Parameters();

            Action act = () => p.AddRequired("", "value");
            act.Should().Throw<ArgumentNullException>();

            act = () => p.AddRequired(null, "value");
            act.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void AddRequired_prohibits_duplicates()
        {
            var p = new Parameters();

            p.Add("key", "value");
            
            Action act = () => p.AddRequired("key", "value2", allowDuplicates: false);
            act.Should().Throw<InvalidOperationException>();
        }
        
        [Fact]
        public void AddRequired_prohibits_empty_value()
        {
            var p = new Parameters();

            Action act = () => p.AddRequired("key", "", allowEmpty: false);
            act.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        public void AddRequired_allows_empty_value()
        {
            var p = new Parameters();

            Action act = () => p.AddRequired("key", "", allowEmpty: true);
            act.Should().NotThrow();
        }
    }
}