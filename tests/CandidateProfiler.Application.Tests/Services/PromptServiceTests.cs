using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Constants;
using CandidateProfiler.Application.Services;
using FluentAssertions;

namespace CandidateProfiler.Application.Tests.Services;

public class PromptServiceTests
{
    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        public PromptService BuildSut()
        {
            return _fixture.Create<PromptService>();
        }

        public string CreateTemplateWithToken()
        {
            var prefix = _fixture.Create<string>();
            var suffix = _fixture.Create<string>();
            return $"{prefix} {PromptTokens.ProcessedText} {suffix}";
        }

        public string CreateTemplateWithMultipleTokens()
        {
            var prefix = _fixture.Create<string>();
            var middle = _fixture.Create<string>();
            var suffix = _fixture.Create<string>();
            return $"{prefix} {PromptTokens.ProcessedText} {middle} {PromptTokens.ProcessedText} {suffix}";
        }

        public string CreateTemplateWithoutToken()
        {
            return _fixture.Create<string>().Replace(PromptTokens.ProcessedText, "");
        }

        public string CreateProcessedText()
        {
            return _fixture.Create<string>();
        }
    }

    #endregion

    [Test]
    public void Given_TemplateWithToken_When_PreparePromptIsCalled_Then_ReplacesTokenWithProcessedText()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var template = builder.CreateTemplateWithToken();
        var processedText = builder.CreateProcessedText();

        var result = sut.PreparePrompt(template, processedText);

        result.Should().NotContain(PromptTokens.ProcessedText);
        result.Should().Contain(processedText);
    }

    [Test]
    public void Given_TemplateWithoutToken_When_PreparePromptIsCalled_Then_ReturnsUnchangedTemplate()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var template = builder.CreateTemplateWithoutToken();
        var processedText = builder.CreateProcessedText();

        var result = sut.PreparePrompt(template, processedText);

        result.Should().Be(template);
    }

    [Test]
    public void Given_TemplateWithMultipleTokens_When_PreparePromptIsCalled_Then_ReplacesAllTokens()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var template = builder.CreateTemplateWithMultipleTokens();
        var processedText = builder.CreateProcessedText();

        var result = sut.PreparePrompt(template, processedText);

        result.Should().NotContain(PromptTokens.ProcessedText);
        result.Should().Contain(processedText);
    }

    [Test]
    public void Given_EmptyTemplate_When_PreparePromptIsCalled_Then_ReturnsEmptyString()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var processedText = builder.CreateProcessedText();

        var result = sut.PreparePrompt(string.Empty, processedText);

        result.Should().Be(string.Empty);
    }

    [Test]
    public void Given_EmptyProcessedText_When_PreparePromptIsCalled_Then_RemovesToken()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var template = builder.CreateTemplateWithToken();

        var result = sut.PreparePrompt(template, string.Empty);

        result.Should().NotContain(PromptTokens.ProcessedText);
    }
}
