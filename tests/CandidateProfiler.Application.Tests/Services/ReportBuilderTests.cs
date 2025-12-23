using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services;
using CandidateProfiler.Application.Services.Abstractions;
using FluentAssertions;
using Moq;

namespace CandidateProfiler.Application.Tests.Services;

public class ReportBuilderTests
{
    private const int DefaultFitValue = 0;
    private const string DefaultBadgeClass = "bg-secondary";
    private const string EmptyIndustryBadgeClass = "bg-secondary";
    private const string DefaultIndustryBadgeClass = "bg-dark";

    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;
        private readonly Mock<ITemplateService> _templateServiceMock;
        private readonly Mock<ITemplateConfigLoader> _templateConfigLoaderMock;
        private TemplateConfig _templateConfig;
        private AppConfig _appConfig;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _templateServiceMock = _fixture.Freeze<Mock<ITemplateService>>();
            _templateConfigLoaderMock = _fixture.Freeze<Mock<ITemplateConfigLoader>>();
            _templateConfig = CreateDefaultTemplateConfig();
            _appConfig = CreateDefaultAppConfig();

            _templateConfigLoaderMock
                .Setup(x => x.LoadConfig(It.IsAny<string>()))
                .Returns(_templateConfig);
        }

        public Mock<ITemplateService> TemplateServiceMock => _templateServiceMock;

        public Builder WithTemplateConfig(TemplateConfig config)
        {
            _templateConfig = config;
            _templateConfigLoaderMock
                .Setup(x => x.LoadConfig(It.IsAny<string>()))
                .Returns(_templateConfig);
            return this;
        }

        public Builder WithSeniorityBadge(string seniority, string badge)
        {
            _templateConfig.BadgeMappings.Seniority[seniority.ToLower()] = badge;
            return this;
        }

        public Builder WithIndustryBadge(string industry, string badge)
        {
            _templateConfig.BadgeMappings.Industry[industry] = badge;
            return this;
        }

        public Builder WithEnglishBadge(string level, string badge)
        {
            _templateConfig.BadgeMappings.EnglishLevel[level.ToLower()] = badge;
            return this;
        }

        public Builder WithFitMapping(string fit, int value)
        {
            _templateConfig.FitMappings[fit.ToLower()] = value;
            return this;
        }

        public ReportBuilder BuildSut()
        {
            return new ReportBuilder(_templateServiceMock.Object, _templateConfigLoaderMock.Object, _appConfig);
        }

        private TemplateConfig CreateDefaultTemplateConfig()
        {
            return new TemplateConfig
            {
                BadgeMappings = new BadgeMappings
                {
                    Seniority = new Dictionary<string, string>
                    {
                        { "default", DefaultBadgeClass }
                    },
                    Industry = new Dictionary<string, string>
                    {
                        { "default", DefaultIndustryBadgeClass },
                        { "empty", EmptyIndustryBadgeClass }
                    },
                    EnglishLevel = new Dictionary<string, string>
                    {
                        { "default", DefaultBadgeClass }
                    }
                },
                FitMappings = new Dictionary<string, int>
                {
                    { "default", DefaultFitValue }
                }
            };
        }

        private AppConfig CreateDefaultAppConfig()
        {
            return new AppConfig
            {
                ConfigFiles = new ConfigFilesConfig
                {
                    TemplateConfig = _fixture.Create<string>()
                },
                Templates = new TemplatesConfig
                {
                    Report = _fixture.Create<string>(),
                    CandidateCard = _fixture.Create<string>(),
                    ChartScript = _fixture.Create<string>()
                }
            };
        }

        public string CreateSeniorityLevel()
        {
            return _fixture.Create<string>();
        }

        public string CreateBadgeClass()
        {
            return _fixture.Create<string>();
        }

        public string CreateFitLevel()
        {
            return _fixture.Create<string>();
        }

        public int CreateFitValue()
        {
            return _fixture.Create<int>();
        }
    }

    #endregion

    [Test]
    public void Given_ReportBuilder_When_Instantiated_Then_LoadsTemplateConfig()
    {
        var builder = new Builder();

        var sut = builder.BuildSut();

        sut.Should().NotBeNull();
    }
}
