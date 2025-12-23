using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Services;
using FluentAssertions;

namespace CandidateProfiler.Application.Tests.Services;

public class PromptLoaderTests
{
    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        public PromptLoader BuildSut()
        {
            return _fixture.Create<PromptLoader>();
        }

        public string CreateTokenKey()
        {
            return _fixture.Create<string>();
        }

        public string CreateTokenValue()
        {
            return _fixture.Create<string>();
        }

        public string CreatePromptWithToken(string tokenKey)
        {
            var prefix = _fixture.Create<string>();
            var suffix = _fixture.Create<string>();
            return $"{prefix} _@{{{tokenKey}}} {suffix}";
        }

        public string CreatePromptWithMultipleTokens(string tokenKey1, string tokenKey2)
        {
            var prefix = _fixture.Create<string>();
            var middle = _fixture.Create<string>();
            var suffix = _fixture.Create<string>();
            return $"{prefix} _@{{{tokenKey1}}} {middle} _@{{{tokenKey2}}} {suffix}";
        }

        public string CreatePromptWithoutTokens()
        {
            return _fixture.Create<string>().Replace("_@{", "").Replace("}", "");
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
    public void Given_PromptWithToken_When_ReplaceTokensIsCalled_Then_ReplacesTokenWithValue()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey = builder.CreateTokenKey();
        var tokenValue = builder.CreateTokenValue();
        var prompt = builder.CreatePromptWithToken(tokenKey);
        var replacements = builder.CreateSingleReplacement(tokenKey, tokenValue);

        var result = sut.ReplaceTokens(prompt, replacements);

        result.Should().Contain(tokenValue);
        result.Should().NotContain($"_@{{{tokenKey}}}");
    }

    [Test]
    public void Given_PromptWithMultipleTokens_When_ReplaceTokensIsCalled_Then_ReplacesAllTokens()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey1 = builder.CreateTokenKey();
        var tokenValue1 = builder.CreateTokenValue();
        var tokenKey2 = builder.CreateTokenKey();
        var tokenValue2 = builder.CreateTokenValue();
        var prompt = builder.CreatePromptWithMultipleTokens(tokenKey1, tokenKey2);
        var replacements = builder.CreateReplacements(tokenKey1, tokenValue1, tokenKey2, tokenValue2);

        var result = sut.ReplaceTokens(prompt, replacements);

        result.Should().Contain(tokenValue1);
        result.Should().Contain(tokenValue2);
        result.Should().NotContain($"_@{{{tokenKey1}}}");
        result.Should().NotContain($"_@{{{tokenKey2}}}");
    }

    [Test]
    public void Given_PromptWithoutTokens_When_ReplaceTokensIsCalled_Then_ReturnsUnchangedPrompt()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey = builder.CreateTokenKey();
        var tokenValue = builder.CreateTokenValue();
        var prompt = builder.CreatePromptWithoutTokens();
        var replacements = builder.CreateSingleReplacement(tokenKey, tokenValue);

        var result = sut.ReplaceTokens(prompt, replacements);

        result.Should().Be(prompt);
    }

    [Test]
    public void Given_EmptyReplacements_When_ReplaceTokensIsCalled_Then_ReturnsUnchangedPrompt()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey = builder.CreateTokenKey();
        var prompt = builder.CreatePromptWithToken(tokenKey);
        var emptyReplacements = new Dictionary<string, string>();

        var result = sut.ReplaceTokens(prompt, emptyReplacements);

        result.Should().Be(prompt);
    }

    [Test]
    public void Given_EmptyPrompt_When_ReplaceTokensIsCalled_Then_ReturnsEmptyString()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var tokenKey = builder.CreateTokenKey();
        var tokenValue = builder.CreateTokenValue();
        var replacements = builder.CreateSingleReplacement(tokenKey, tokenValue);

        var result = sut.ReplaceTokens(string.Empty, replacements);

        result.Should().Be(string.Empty);
    }
}
