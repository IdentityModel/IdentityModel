using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.Json;
using FluentAssertions;
using IdentityModel.Client;
using Xunit;

public class JsonElementTests
{
    public static IEnumerable<object[]> StringTestCases()
    {
        yield return new object[] { "string" };
        yield return new object[] { "some other string with spaces" };
        yield return new object[] { string.Empty };
    }

    [Fact]
    public void AsJsonElement_should_succeed_for_null_strings()
    {
        string s = null;
        var json = s.AsJsonElement();
        json.ValueKind.Should().Be(JsonValueKind.Null);
    }

    [Theory]
    [MemberData(nameof(StringTestCases))]
    public void AsJsonElement_should_succeed_for_strings(string s)
    {
        var json = s.AsJsonElement();
        json.ValueKind.Should().Be(JsonValueKind.String);
        json.GetString().Should().Be(s);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AsJsonElement_should_succeed_for_bools(bool b)
    {
        var json = b.AsJsonElement();
        json.ValueKind.Should().Be(b ? JsonValueKind.True : JsonValueKind.False);
        json.GetBoolean().Should().Be(b);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void AsJsonElement_should_succeed_for_ints(int x)
    {
        var json = x.AsJsonElement();
        json.ValueKind.Should().Be(JsonValueKind.Number);
        json.GetInt32().Should().Be(x);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public void AsJsonElement_should_succeed_for_longs(long x)
    {
        var json = x.AsJsonElement();
        json.ValueKind.Should().Be(JsonValueKind.Number);
        json.GetInt64().Should().Be(x);
    }
}