using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services;
using FluentAssertions;

namespace CandidateProfiler.Application.Tests.Services;

public class AppConfigLoaderTests
{
    private const string OllamaProviderValue = "Ollama";
    private const string OpenAiProviderValue = "OpenAi";
    private const string InvalidJsonContent = "{ invalid json }";
    private const string NullJsonContent = "null";

    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;
        private string _tempFilePath;
        private AppConfig _appConfig;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _tempFilePath = Path.GetTempFileName();
            _appConfig = CreateDefaultAppConfig();
        }

        public Builder WithLlmProvider(string provider)
        {
            _appConfig.LlmProvider = provider;
            return this;
        }

        public Builder WithAppConfig(AppConfig config)
        {
            _appConfig = config;
            return this;
        }

        public AppConfigLoader BuildSut()
        {
            return _fixture.Create<AppConfigLoader>();
        }

        public string CreateConfigFile()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(_appConfig, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_tempFilePath, json);
            return _tempFilePath;
        }

        public string CreateInvalidJsonFile()
        {
            var invalidPath = Path.GetTempFileName();
            File.WriteAllText(invalidPath, InvalidJsonContent);
            return invalidPath;
        }

        public string CreateNullJsonFile()
        {
            var nullPath = Path.GetTempFileName();
            File.WriteAllText(nullPath, NullJsonContent);
            return nullPath;
        }

        public string CreateNonExistentFilePath()
        {
            return _fixture.Create<string>();
        }

        private AppConfig CreateDefaultAppConfig()
        {
            return new AppConfig
            {
                Paths = new PathsConfig
                {
                    DataRoot = _fixture.Create<string>(),
                    Config = _fixture.Create<string>(),
                    Templates = _fixture.Create<string>(),
                    Input = _fixture.Create<string>(),
                    Output = _fixture.Create<string>(),
                    Temp = _fixture.Create<string>()
                },
                ConfigFiles = new ConfigFilesConfig
                {
                    Prompt = _fixture.Create<string>(),
                    Task = _fixture.Create<string>(),
                    LlmConfig = _fixture.Create<string>(),
                    TemplateConfig = _fixture.Create<string>()
                },
                Templates = new TemplatesConfig
                {
                    Report = _fixture.Create<string>(),
                    CandidateCard = _fixture.Create<string>(),
                    ChartScript = _fixture.Create<string>()
                },
                LlmProvider = OllamaProviderValue,
                Ollama = new OllamaConfig
                {
                    BaseUrl = CreateUrl(),
                    ModelName = _fixture.Create<string>(),
                    Temperature = _fixture.Create<double>(),
                    TimeoutMinutes = _fixture.Create<int>()
                },
                OpenAi = new OpenAiConfig
                {
                    BaseUrl = CreateUrl(),
                    ModelName = _fixture.Create<string>(),
                    ApiKey = _fixture.Create<string>(),
                    Temperature = _fixture.Create<double>(),
                    TimeoutMinutes = _fixture.Create<int>(),
                    DelayMilliseconds = _fixture.Create<int>()
                }
            };
        }

        private string CreateUrl()
        {
            return $"http://{_fixture.Create<string>()}";
        }

        public void Cleanup()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }

        public void CleanupFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    #endregion

    [Test]
    public void Given_ConfigFileWithOllamaProvider_When_LoadConfigIsCalled_Then_LoadsOllamaProvider()
    {
        var builder = new Builder().WithLlmProvider(OllamaProviderValue);
        var sut = builder.BuildSut();
        var configPath = builder.CreateConfigFile();

        var result = sut.LoadConfig(configPath);

        result.Should().NotBeNull();
        result.LlmProvider.Should().Be(OllamaProviderValue);
        builder.Cleanup();
    }

    [Test]
    public void Given_ConfigFileWithOpenAiProvider_When_LoadConfigIsCalled_Then_LoadsOpenAiProvider()
    {
        var builder = new Builder().WithLlmProvider(OpenAiProviderValue);
        var sut = builder.BuildSut();
        var configPath = builder.CreateConfigFile();

        var result = sut.LoadConfig(configPath);

        result.Should().NotBeNull();
        result.LlmProvider.Should().Be(OpenAiProviderValue);
        builder.Cleanup();
    }

    [Test]
    public void Given_ValidConfigFile_When_LoadConfigIsCalled_Then_LoadsAllProperties()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var configPath = builder.CreateConfigFile();

        var result = sut.LoadConfig(configPath);

        result.Should().NotBeNull();
        result.Paths.Should().NotBeNull();
        result.ConfigFiles.Should().NotBeNull();
        result.Templates.Should().NotBeNull();
        result.Ollama.Should().NotBeNull();
        result.OpenAi.Should().NotBeNull();
        builder.Cleanup();
    }

    [Test]
    public void Given_NonExistentFile_When_LoadConfigIsCalled_Then_ThrowsFileNotFoundException()
    {
        var builder = new Builder();
        var nonExistentPath = builder.CreateNonExistentFilePath();
        var sut = builder.BuildSut();

        Action act = () => sut.LoadConfig(nonExistentPath);

        act.Should().Throw<FileNotFoundException>();
    }

    [Test]
    public void Given_InvalidJsonFile_When_LoadConfigIsCalled_Then_ThrowsJsonException()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var invalidJsonPath = builder.CreateInvalidJsonFile();

        Action act = () => sut.LoadConfig(invalidJsonPath);

        act.Should().Throw<Newtonsoft.Json.JsonReaderException>();
        builder.CleanupFile(invalidJsonPath);
    }

    [Test]
    public void Given_EmptyJsonFile_When_LoadConfigIsCalled_Then_ThrowsInvalidOperationException()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();
        var emptyJsonPath = builder.CreateNullJsonFile();

        Action act = () => sut.LoadConfig(emptyJsonPath);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Failed to load application config from {emptyJsonPath}");
        builder.CleanupFile(emptyJsonPath);
    }
}