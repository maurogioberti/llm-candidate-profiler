using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Services;
using FluentAssertions;

namespace CandidateProfiler.Application.Tests.Services;

public class TemplateServiceTests
{
    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        public TemplateService BuildSut()
        {
            return _fixture.Create<TemplateService>();
        }

        public string CreateTokenKey()
        {
            return _fixture.Create<string>();
        }

        public string CreateTokenValue()
        {
            return _fixture.Create<string>();
        }

        public string CreateTemplateWithTokens(string tokenKey1, string tokenKey2)
        {
            var prefix = _fixture.Create<string>();
            var middle = _fixture.Create<string>();
            var suffix = _fixture.Create<string>();
            return $"{prefix} {{{{{tokenKey1}}}}}{middle}{{{{{tokenKey2}}}}}{suffix}";
        }

        public string CreateTemplateWithSingleToken(string tokenKey)
        {
            return $"{{{{{tokenKey}}}}} {_fixture.Create<string>()} {{{{{tokenKey}}}}} {_fixture.Create<string>()}";
        }

        public string CreateTemplateWithoutTokens()
        {
            return _fixture.Create<string>().Replace("{{", "").Replace("}}", "");
        }

        public Dictionary<string, string> CreateReplacements(string key1, string value1, string key2, string value2)
        {
            return new Dictionary<string, string>
            {
                { key1, value1 },
                { key2, value2 }
            };
        }

        public Dictionary<string, string> CreateSingleReplacement(string key, string value)
        {
            return new Dictionary<string, string> { { key, value } };
        }
    }

    #endregion

    [Test]
    public void Given_TemplateWithTokens_When_ReplaceTokensIsCalled_Then_ReplacesAllTokens()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey1 = builder.CreateTokenKey();
        var tokenValue1 = builder.CreateTokenValue();
        var tokenKey2 = builder.CreateTokenKey();
        var tokenValue2 = builder.CreateTokenValue();
        var template = builder.CreateTemplateWithTokens(tokenKey1, tokenKey2);
        var replacements = builder.CreateReplacements(tokenKey1, tokenValue1, tokenKey2, tokenValue2);

        var result = sut.ReplaceTokens(template, replacements);

        result.Should().Contain(tokenValue1);
        result.Should().Contain(tokenValue2);
        result.Should().NotContain($"{{{{{tokenKey1}}}}}");
        result.Should().NotContain($"{{{{{tokenKey2}}}}}");
    }

    [Test]
    public void Given_TemplateWithoutTokens_When_ReplaceTokensIsCalled_Then_ReturnsUnchangedTemplate()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey1 = builder.CreateTokenKey();
        var tokenValue1 = builder.CreateTokenValue();
        var tokenKey2 = builder.CreateTokenKey();
        var tokenValue2 = builder.CreateTokenValue();
        var template = builder.CreateTemplateWithoutTokens();
        var replacements = builder.CreateReplacements(tokenKey1, tokenValue1, tokenKey2, tokenValue2);

        var result = sut.ReplaceTokens(template, replacements);

        result.Should().Be(template);
    }

    [Test]
    public void Given_EmptyReplacements_When_ReplaceTokensIsCalled_Then_ReturnsUnchangedTemplate()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey1 = builder.CreateTokenKey();
        var tokenKey2 = builder.CreateTokenKey();
        var template = builder.CreateTemplateWithTokens(tokenKey1, tokenKey2);
        var emptyReplacements = new Dictionary<string, string>();

        var result = sut.ReplaceTokens(template, emptyReplacements);

        result.Should().Be(template);
    }

    [Test]
    public void Given_TemplateWithMultipleSameTokens_When_ReplaceTokensIsCalled_Then_ReplacesAllOccurrences()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey = builder.CreateTokenKey();
        var tokenValue = builder.CreateTokenValue();
        var template = builder.CreateTemplateWithSingleToken(tokenKey);
        var replacements = builder.CreateSingleReplacement(tokenKey, tokenValue);

        var result = sut.ReplaceTokens(template, replacements);

        result.Should().Contain(tokenValue);
        result.Should().NotContain($"{{{{{tokenKey}}}}}");
    }

    [Test]
    public void Given_PartialReplacements_When_ReplaceTokensIsCalled_Then_ReplacesOnlyMatchingTokens()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey1 = builder.CreateTokenKey();
        var tokenValue1 = builder.CreateTokenValue();
        var tokenKey2 = builder.CreateTokenKey();
        var template = builder.CreateTemplateWithTokens(tokenKey1, tokenKey2);
        var partialReplacements = builder.CreateSingleReplacement(tokenKey1, tokenValue1);

        var result = sut.ReplaceTokens(template, partialReplacements);

        result.Should().Contain(tokenValue1);
        result.Should().NotContain($"{{{{{tokenKey1}}}}}");
        result.Should().Contain($"{{{{{tokenKey2}}}}}");
    }

    [Test]
    public void Given_EmptyTemplate_When_ReplaceTokensIsCalled_Then_ReturnsEmptyString()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey1 = builder.CreateTokenKey();
        var tokenValue1 = builder.CreateTokenValue();
        var tokenKey2 = builder.CreateTokenKey();
        var tokenValue2 = builder.CreateTokenValue();
        var replacements = builder.CreateReplacements(tokenKey1, tokenValue1, tokenKey2, tokenValue2);

        var result = sut.ReplaceTokens(string.Empty, replacements);

        result.Should().Be(string.Empty);
    }
}
