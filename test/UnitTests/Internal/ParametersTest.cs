using FluentAssertions;
using System;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class ParametersTest
    {
        [Fact]
        public void AddOptional_empty_key_should_fail()
        {
            var key = "";
            var value = "custom";
            var parameters = new Parameters();

            Action act = () => parameters.AddOptional(key, value);
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("key");
        }

        [Fact]
        public void AddOptional_with_empty_value_should_not_be_added()
        {
	        var key = "custom";
	        var value = "";
	        var parameters = new Parameters();
	        
	        parameters.AddOptional(key, value);
	        parameters.Should().BeEmpty();
        }

        [Fact]
        public void AddOptional_with_duplicate_key_should_fail()
        {
	        var key = "custom";
	        var value = "custom";
	        var parameters = new Parameters();
	        
	        parameters.AddOptional(key, value);

	        Action act = () => parameters.AddOptional(key, value);
	        act.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Duplicate parameter: {key}");
        }

        [Fact]
        public void AddRequired_empty_key_should_fail()
        {
	        var key = "";
	        var value = "custom";
	        var parameters = new Parameters();

	        Action act = () => parameters.AddRequired(key, value);
	        act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("key");
        }

        [Fact]
        public void AddRequired_with_empty_value_should_fail()
        {
	        var key = "custom";
	        var value = "";
	        var parameters = new Parameters();
	        
	        Action act = () => parameters.AddRequired(key, value);
	        act.Should().Throw<ArgumentException>().And.ParamName.Should().Be(key);
        }

        [Fact]
        public void AddRequired_with_empty_value_with_allowing_empty_should_be_added()
        {
	        var key = "custom";
	        var value = "";
	        var parameters = new Parameters();
	        
	        parameters.AddRequired(key, value, allowEmptyValue: true);
	        parameters.Should().HaveCount(1);
        }

        [Fact]
        public void AddRequired_with_duplicate_key_should_fail()
        {
	        var key = "custom";
	        var value = "custom";
	        var parameters = new Parameters();
	        
	        parameters.AddRequired(key, value);

	        Action act = () => parameters.AddRequired(key, value);
	        act.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Duplicate parameter: {key}");
        }
        
        [Fact]
        public void Default_add_does_not_replace()
        {
	        var key = "custom";
	        var value = "custom";
	        var parameters = new Parameters();

	        parameters.Add(key, value);
	        parameters.Add(key, value);

	        parameters.Should().HaveCount(2);
        }
        
        [Fact]
        public void Add_with_single_replace_works_as_expected()
        {
	        var key = "custom";
	        var value = "custom";
	        var parameters = new Parameters();

	        parameters.Add(key, value);
	        parameters.Add(key, value, ParameterReplaceBehavior.Single);

	        parameters.Should().HaveCount(1);
        }
        
        [Fact]
        public void Add_with_all_replace_works_as_expected()
        {
	        var key = "custom";
	        var value = "custom";
	        var parameters = new Parameters();

	        parameters.Add(key, value);
	        parameters.Add(key, value, ParameterReplaceBehavior.All);

	        parameters.Should().HaveCount(1);
        }
        
        [Fact]
        public void Add_with_single_replace_but_multiple_exist_should_throw()
        {
	        var key = "custom";
	        var parameters = new Parameters();

	        parameters.Add(key, "value1");
	        parameters.Add(key, "value2");
	        
	        Action act = () => parameters.Add(key,"value3", ParameterReplaceBehavior.Single);
	        act.Should().Throw<InvalidOperationException>();
        }
    }
}
