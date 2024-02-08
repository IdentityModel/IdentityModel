using FluentAssertions;
using System;
using IdentityModel.Client;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class ParametersTest
    {
        private const string Key = "custom";
        private const string Value = "custom";

        private readonly Parameters Parameters = new Parameters();

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddOptional_with_missing_key_should_fail(string missingKey)
        {
            Action act = () => Parameters.AddOptional(missingKey, Value);
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("key");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddOptional_with_empty_value_should_not_be_added(string emptyValue)
        {
            Parameters.AddOptional(Key, emptyValue);
            Parameters.Should().BeEmpty();
        }

        [Fact]
        public void AddOptional_with_key_and_value_should_add()
        {
            Parameters.AddOptional(Key, Value);
            Parameters.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(Value)]
        [InlineData("different value")]
        public void AddOptional_with_duplicate_key_should_fail(string value)
        {
            Parameters.AddOptional(Key, Value);
            Action act = () => Parameters.AddOptional(Key, value);
            act.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Duplicate parameter: {Key}");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddOptional_with_duplicate_key_without_a_value_should_noop(string emptyValue)
        {
            Parameters.Add(Key, Value);
            Parameters.AddOptional(Key, emptyValue);
            Parameters.Should().HaveCount(1);
        }

        [Fact]
        public void AddOptional_with_allow_duplicates_should_add_values()
        {
            Parameters.Add(Key, Value);
            Parameters.AddOptional(Key, "new value", allowDuplicates: true);
            Parameters.Should().HaveCount(2);
            Parameters.GetValues(Key).Should().HaveCount(2);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddOptional_with_allow_duplicates_should_not_add_empty_value(string emptyValue)
        {
            var parameters = new Parameters
            {
                { Key, Value}
            };
            
            parameters.AddOptional(Key, emptyValue, allowDuplicates: true);

            parameters.Should().HaveCount(1);
            parameters.GetValues(Key).Should().HaveCount(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddRequired_with_missing_key_should_fail(string missingKey)
        {
            Action act = () => Parameters.AddRequired(missingKey, Value);
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("key");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddRequired_with_empty_value_should_fail(string emptyValue)
        {
            Action act = () => Parameters.AddRequired(Key, emptyValue);
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be(Key);
        }

        [Fact]
        public void AddRequired_with_key_and_value_should_add()
        {
            Parameters.AddRequired(Key, Value);
            Parameters.Should().HaveCount(1);
        }

        [Fact]
        public void AddRequired_with_empty_value_and_existing_parameter_should_noop()
        {
            var parameters = new Parameters();
            parameters.AddRequired(Key, Value);
            parameters.AddRequired(Key, null);
            parameters.AddRequired(Key, "");

            parameters.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddRequired_with_empty_value_and_allowEmptyValue_should_add(string emptyValue)
        {
            var parameters = new Parameters();
            
            parameters.AddRequired(Key, emptyValue, allowEmptyValue: true);
            parameters.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        // This test name is a mouthful! We want to make sure that we can add a
        // duplicate empty value by setting the allowDuplicates and
        // allowEmptyValue parameters.
        public void AddRequired_with_duplicate_empty_value_and_allowEmptyValue_and_allowDuplicates_should_add(string emptyValue)
        {
            var parameters = new Parameters
            {
                { Key, Value}
            };
            parameters.AddRequired(Key, emptyValue, allowDuplicates: true, allowEmptyValue: true);

            parameters.Should().HaveCount(2);
            parameters[Key].Should().HaveCount(2);
        }

        [Fact]
        public void AddRequired_with_duplicate_key_and_distinct_values_should_fail()
        {
            var parameters = new Parameters
            {
                { Key, Value}
            };

            Action act = () => parameters.AddRequired(Key, "new value");
            act.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Duplicate parameter: {Key}");
        }

        [Fact]
        public void AddRequired_with_duplicate_key_and_value_should_noop()
        {
            Parameters.AddRequired(Key, Value);
            Parameters.AddRequired(Key, Value);

            Parameters.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AddRequired_with_duplicate_key_without_a_value_should_noop(string emptyValue)
        {
            Parameters.Add(Key, Value);
            Parameters.AddRequired(Key, emptyValue);
            Parameters.Should().HaveCount(1);
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
