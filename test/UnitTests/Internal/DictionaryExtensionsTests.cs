using FluentAssertions;
using IdentityModel.Internal;
using System;
using System.Collections.Generic;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void AddOptional_empty_key_should_fail()
        {
            var key = "";
            var value = "custom";
            var parameters = new Dictionary<string, string>();

            Action act = () => parameters.AddOptional(key, value);
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("key");
        }

        [Fact]
        public void AddOptional_with_empty_value_should_not_be_added()
        {
	        var key = "custom";
	        var value = "";
	        var parameters = new Dictionary<string, string>();
	        parameters.AddOptional(key, value);
	        parameters.Should().BeEmpty();
        }

        [Fact]
        public void AddOptional_with_duplicate_key_should_fail()
        {
	        var key = "custom";
	        var value = "custom";
	        var parameters = new Dictionary<string, string>();
	        parameters.AddOptional(key, value);

	        Action act = () => parameters.AddOptional(key, value);
	        act.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Duplicate parameter: {key}");
        }

        [Fact]
        public void AddRequired_empty_key_should_fail()
        {
	        var key = "";
	        var value = "custom";
	        var parameters = new Dictionary<string, string>();

	        Action act = () => parameters.AddRequired(key, value);
	        act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("key");
        }

        [Fact]
        public void AddRequired_with_empty_value_should_fail()
        {
	        var key = "custom";
	        var value = "";
	        var parameters = new Dictionary<string, string>();
	        Action act = () => parameters.AddRequired(key, value);
	        act.Should().Throw<ArgumentException>().And.ParamName.Should().Be(key);
        }

        [Fact]
        public void AddRequired_with_empty_value_with_allowing_empty_should_be_added()
        {
	        var key = "custom";
	        var value = "";
	        var parameters = new Dictionary<string, string>();
	        parameters.AddRequired(key, value, true);
	        parameters.Should().HaveCount(1);
        }

        [Fact]
        public void AddRequired_with_duplicate_key_should_fail()
        {
	        var key = "custom";
	        var value = "custom";
	        var parameters = new Dictionary<string, string>();
	        parameters.AddRequired(key, value);

	        Action act = () => parameters.AddRequired(key, value);
	        act.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Duplicate parameter: {key}");
        }
    }
}
