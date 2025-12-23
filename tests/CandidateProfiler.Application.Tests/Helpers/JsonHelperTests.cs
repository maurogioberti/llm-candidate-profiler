using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Helpers;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace CandidateProfiler.Application.Tests.Helpers;

public class JsonHelperTests
{
    private const string EmptyJsonObject = @"{}";
    private const int InitialPageNumber = 0;

    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        public string CreateJsonObject(string propertyKey1, string propertyValue1, string propertyKey2, int propertyValue2)
        {
            return $@"{{""{propertyKey1}"":""{propertyValue1}"",""{propertyKey2}"":{propertyValue2}}}";
        }

        public string CreateJsonWithPrefix(string json)
        {
            var prefix = _fixture.Create<string>();
            return $"{prefix} {json}";
        }

        public string CreateJsonWithSuffix(string json)
        {
            var suffix = _fixture.Create<string>();
            return $"{json} {suffix}";
        }

        public string CreateJsonWithPrefixAndSuffix(string json)
        {
            var prefix = _fixture.Create<string>();
            var suffix = _fixture.Create<string>();
            return $"{prefix} {json} {suffix}";
        }

        public string CreateNestedJson(string outerKey, string innerKey, string innerValue)
        {
            return $@"{{""{outerKey}"":{{""{innerKey}"":""{innerValue}""}}}}";
        }

        public string CreateInvalidJsonMalformed()
        {
            var content = _fixture.Create<string>();
            return $"{{{content}}}";
        }

        public string CreatePropertyKey()
        {
            return _fixture.Create<string>();
        }

        public string CreatePropertyValue()
        {
            return _fixture.Create<string>();
        }

        public int CreateIntValue()
        {
            return _fixture.Create<int>();
        }

        public string CreateTextWithoutBraces()
        {
            return _fixture.Create<string>().Replace("{", "").Replace("}", "");
        }
    }

    #endregion

    [Test]
    public void Given_ValidJsonString_When_TryParseJsonIsCalled_Then_ReturnsJObject()
    {
        var builder = new Builder();
        var propertyKey1 = builder.CreatePropertyKey();
        var propertyValue1 = builder.CreatePropertyValue();
        var propertyKey2 = builder.CreatePropertyKey();
        var propertyValue2 = builder.CreateIntValue();
        var validJson = builder.CreateJsonObject(propertyKey1, propertyValue1, propertyKey2, propertyValue2);

        var result = JsonHelper.TryParseJson(validJson);

        result.Should().NotBeNull();
        result![propertyKey1]!.ToString().Should().Be(propertyValue1);
        result[propertyKey2]!.ToString().Should().Be(propertyValue2.ToString());
    }

    [Test]
    public void Given_JsonWithPrefix_When_TryParseJsonIsCalled_Then_ExtractsAndReturnsJObject()
    {
        var builder = new Builder();
        var propertyKey1 = builder.CreatePropertyKey();
        var propertyValue1 = builder.CreatePropertyValue();
        var propertyKey2 = builder.CreatePropertyKey();
        var propertyValue2 = builder.CreateIntValue();
        var validJson = builder.CreateJsonObject(propertyKey1, propertyValue1, propertyKey2, propertyValue2);
        var jsonWithPrefix = builder.CreateJsonWithPrefix(validJson);

        var result = JsonHelper.TryParseJson(jsonWithPrefix);

        result.Should().NotBeNull();
        result![propertyKey1]!.ToString().Should().Be(propertyValue1);
        result[propertyKey2]!.ToString().Should().Be(propertyValue2.ToString());
    }

    [Test]
    public void Given_JsonWithSuffix_When_TryParseJsonIsCalled_Then_ExtractsAndReturnsJObject()
    {
        var builder = new Builder();
        var propertyKey1 = builder.CreatePropertyKey();
        var propertyValue1 = builder.CreatePropertyValue();
        var propertyKey2 = builder.CreatePropertyKey();
        var propertyValue2 = builder.CreateIntValue();
        var validJson = builder.CreateJsonObject(propertyKey1, propertyValue1, propertyKey2, propertyValue2);
        var jsonWithSuffix = builder.CreateJsonWithSuffix(validJson);

        var result = JsonHelper.TryParseJson(jsonWithSuffix);

        result.Should().NotBeNull();
        result![propertyKey1]!.ToString().Should().Be(propertyValue1);
        result[propertyKey2]!.ToString().Should().Be(propertyValue2.ToString());
    }

    [Test]
    public void Given_JsonWithPrefixAndSuffix_When_TryParseJsonIsCalled_Then_ExtractsAndReturnsJObject()
    {
        var builder = new Builder();
        var propertyKey1 = builder.CreatePropertyKey();
        var propertyValue1 = builder.CreatePropertyValue();
        var propertyKey2 = builder.CreatePropertyKey();
        var propertyValue2 = builder.CreateIntValue();
        var validJson = builder.CreateJsonObject(propertyKey1, propertyValue1, propertyKey2, propertyValue2);
        var jsonWithBoth = builder.CreateJsonWithPrefixAndSuffix(validJson);

        var result = JsonHelper.TryParseJson(jsonWithBoth);

        result.Should().NotBeNull();
        result![propertyKey1]!.ToString().Should().Be(propertyValue1);
        result[propertyKey2]!.ToString().Should().Be(propertyValue2.ToString());
    }

    [Test]
    public void Given_InvalidJsonNoBraces_When_TryParseJsonIsCalled_Then_ReturnsNull()
    {
        var builder = new Builder();
        var textWithoutBraces = builder.CreateTextWithoutBraces();

        var result = JsonHelper.TryParseJson(textWithoutBraces);

        result.Should().BeNull();
    }

    [Test]
    public void Given_InvalidJsonMalformed_When_TryParseJsonIsCalled_Then_ReturnsNull()
    {
        var builder = new Builder();
        var invalidJson = builder.CreateInvalidJsonMalformed();

        var result = JsonHelper.TryParseJson(invalidJson);

        result.Should().BeNull();
    }

    [Test]
    public void Given_EmptyJsonObject_When_TryParseJsonIsCalled_Then_ReturnsEmptyJObject()
    {
        var result = JsonHelper.TryParseJson(EmptyJsonObject);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public void Given_NestedJsonObject_When_TryParseJsonIsCalled_Then_ReturnsNestedJObject()
    {
        var builder = new Builder();
        var outerKey = builder.CreatePropertyKey();
        var innerKey = builder.CreatePropertyKey();
        var innerValue = builder.CreatePropertyValue();
        var nestedJson = builder.CreateNestedJson(outerKey, innerKey, innerValue);

        var result = JsonHelper.TryParseJson(nestedJson);

        result.Should().NotBeNull();
        result![outerKey].Should().NotBeNull();
        result[outerKey]![innerKey]!.ToString().Should().Be(innerValue);
    }

    [Test]
    public void Given_NullString_When_TryParseJsonIsCalled_Then_ThrowsNullReferenceException()
    {
        Action act = () => JsonHelper.TryParseJson(null!);

        act.Should().Throw<NullReferenceException>();
    }

    [Test]
    public void Given_EmptyString_When_TryParseJsonIsCalled_Then_ReturnsNull()
    {
        var result = JsonHelper.TryParseJson(string.Empty);

        result.Should().BeNull();
    }

    [Test]
    public void Given_WhitespaceString_When_TryParseJsonIsCalled_Then_ReturnsNull()
    {
        const string whitespaceString = "   ";

        var result = JsonHelper.TryParseJson(whitespaceString);

        result.Should().BeNull();
    }
}
