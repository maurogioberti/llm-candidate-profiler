using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services;
using FluentAssertions;

namespace CandidateProfiler.Application.Tests.Services;

public class LlmServiceFactoryTests
{
    private const string OllamaProviderName = "ollama";
    private const string OpenAiProviderName = "openai";
    private const string OllamaMixedCaseProviderName = "Ollama";
    private const string OpenAiMixedCaseProviderName = "OpenAi";
    private const string HttpClientParameterName = "httpClient";
    private const string AppConfigParameterName = "appConfig";
    private const int DefaultTimeoutMinutes = 5;
    private const int DefaultDelayMilliseconds = 0;

    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;
        private readonly HttpClient _httpClient;
        private AppConfig _appConfig;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _httpClient = new HttpClient();
            _appConfig = CreateDefaultAppConfig();
        }

        public Builder WithLlmProvider(string provider)
        {
            _appConfig.LlmProvider = provider;
            return this;
        }

        public Builder WithNullHttpClient()
        {
            return this;
        }

        public Builder WithNullAppConfig()
        {
            _appConfig = null!;
            return this;
        }

        public LlmServiceFactory BuildSut()
        {
            return new LlmServiceFactory(_httpClient, _appConfig);
        }

        public LlmServiceFactory BuildSutWithNullHttpClient()
        {
            return new LlmServiceFactory(null!, _appConfig);
        }

        public LlmServiceFactory BuildSutWithNullAppConfig()
        {
            return new LlmServiceFactory(_httpClient, null!);
        }

        private AppConfig CreateDefaultAppConfig()
        {
            return new AppConfig
            {
                LlmProvider = OllamaProviderName,
                Ollama = CreateOllamaConfig(),
                OpenAi = CreateOpenAiConfig()
            };
        }

        private OllamaConfig CreateOllamaConfig()
        {
            return new OllamaConfig
            {
                BaseUrl = CreateBaseUrl(),
                ModelName = _fixture.Create<string>(),
                Temperature = _fixture.Create<double>(),
                TimeoutMinutes = DefaultTimeoutMinutes
            };
        }

        private OpenAiConfig CreateOpenAiConfig()
        {
            return new OpenAiConfig
            {
                BaseUrl = CreateBaseUrl(),
                ModelName = _fixture.Create<string>(),
                ApiKey = _fixture.Create<string>(),
                Temperature = _fixture.Create<double>(),
                TimeoutMinutes = DefaultTimeoutMinutes,
                DelayMilliseconds = DefaultDelayMilliseconds
            };
        }

        private string CreateBaseUrl()
        {
            return $"http://{_fixture.Create<string>()}";
        }

        public string CreateInvalidProviderName()
        {
            return _fixture.Create<string>();
        }
    }

    #endregion

    [Test]
    public void Given_OllamaProvider_When_CreateLlmServiceIsCalled_Then_ReturnsOllamaLlmService()
    {
        var builder = new Builder().WithLlmProvider(OllamaProviderName);
        var sut = builder.BuildSut();

        var result = sut.CreateLlmService();

        result.Should().NotBeNull();
        result.Should().BeOfType<OllamaLlmService>();
    }

    [Test]
    public void Given_OpenAiProvider_When_CreateLlmServiceIsCalled_Then_ReturnsOpenAiLlmService()
    {
        var builder = new Builder().WithLlmProvider(OpenAiProviderName);
        var sut = builder.BuildSut();

        var result = sut.CreateLlmService();

        result.Should().NotBeNull();
        result.Should().BeOfType<OpenAiLlmService>();
    }

    [Test]
    public void Given_OllamaMixedCaseProvider_When_CreateLlmServiceIsCalled_Then_ReturnsOllamaLlmService()
    {
        var builder = new Builder().WithLlmProvider(OllamaMixedCaseProviderName);
        var sut = builder.BuildSut();

        var result = sut.CreateLlmService();

        result.Should().NotBeNull();
        result.Should().BeOfType<OllamaLlmService>();
    }

    [Test]
    public void Given_OpenAiMixedCaseProvider_When_CreateLlmServiceIsCalled_Then_ReturnsOpenAiLlmService()
    {
        var builder = new Builder().WithLlmProvider(OpenAiMixedCaseProviderName);
        var sut = builder.BuildSut();

        var result = sut.CreateLlmService();

        result.Should().NotBeNull();
        result.Should().BeOfType<OpenAiLlmService>();
    }

    [Test]
    public void Given_UnsupportedProvider_When_CreateLlmServiceIsCalled_Then_ThrowsInvalidOperationException()
    {
        var builder = new Builder();
        var invalidProvider = builder.CreateInvalidProviderName();
        builder.WithLlmProvider(invalidProvider);
        var sut = builder.BuildSut();

        Action act = () => sut.CreateLlmService();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Unsupported LLM provider: {invalidProvider}*");
    }

    [Test]
    public void Given_NullProvider_When_CreateLlmServiceIsCalled_Then_ThrowsInvalidOperationException()
    {
        var builder = new Builder().WithLlmProvider(null!);
        var sut = builder.BuildSut();

        Action act = () => sut.CreateLlmService();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Unsupported LLM provider:*");
    }

    [Test]
    public void Given_EmptyProvider_When_CreateLlmServiceIsCalled_Then_ThrowsInvalidOperationException()
    {
        var builder = new Builder().WithLlmProvider(string.Empty);
        var sut = builder.BuildSut();

        Action act = () => sut.CreateLlmService();

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void Given_NullHttpClient_When_FactoryIsCreated_Then_ThrowsArgumentNullException()
    {
        var builder = new Builder();

        Action act = () => builder.BuildSutWithNullHttpClient();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName(HttpClientParameterName);
    }

    [Test]
    public void Given_NullAppConfig_When_FactoryIsCreated_Then_ThrowsArgumentNullException()
    {
        var builder = new Builder();

        Action act = () => builder.BuildSutWithNullAppConfig();

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName(AppConfigParameterName);
    }

    [Test]
    public void Given_ValidFactory_When_CreateLlmServiceIsCalledMultipleTimes_Then_CreatesNewInstancesEachTime()
    {
        var builder = new Builder().WithLlmProvider(OllamaProviderName);
        var sut = builder.BuildSut();

        var result1 = sut.CreateLlmService();
        var result2 = sut.CreateLlmService();

        result1.Should().NotBeSameAs(result2);
    }

    [Test]
    public void Given_FactoryImplementsInterface_When_Checked_Then_ImplementsILlmServiceFactory()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();

        sut.Should().BeAssignableTo<ILlmServiceFactory>();
    }
}