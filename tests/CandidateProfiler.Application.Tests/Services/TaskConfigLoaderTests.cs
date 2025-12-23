using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Domain.Models;
using CandidateProfiler.Application.Services;
using CandidateProfiler.Application.Services.Abstractions;
using FluentAssertions;
using Moq;

namespace CandidateProfiler.Application.Tests.Services;

public class TaskConfigLoaderTests
{
    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;
        private readonly Mock<IPromptLoader> _promptLoaderMock;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _promptLoaderMock = _fixture.Freeze<Mock<IPromptLoader>>();
        }

        public Mock<IPromptLoader> PromptLoaderMock => _promptLoaderMock;

        public Builder WithPromptContent(string content)
        {
            _promptLoaderMock
                .Setup(x => x.LoadPrompt(It.IsAny<string>()))
                .Returns(content);
            return this;
        }

        public TaskConfigLoader BuildSut()
        {
            return _fixture.Create<TaskConfigLoader>();
        }

        public RagTaskConfig CreateTaskConfig()
        {
            var promptPath = _fixture.Create<string>();
            return new RagTaskConfig(promptPath);
        }

        public RagTaskConfig CreateTaskConfig(string promptPath)
        {
            return new RagTaskConfig(promptPath);
        }

        public string CreatePromptPath()
        {
            return _fixture.Create<string>();
        }

        public string CreatePromptContent()
        {
            return _fixture.Create<string>();
        }
    }

    #endregion

    [Test]
    public void Given_ValidTaskConfig_When_LoadAssetsAsyncIsCalled_Then_ReturnsLoadedTaskAssets()
    {
        var builder = new Builder();
        var promptContent = builder.CreatePromptContent();
        builder.WithPromptContent(promptContent);
        var sut = builder.BuildSut();
        var taskConfig = builder.CreateTaskConfig();

        var result = sut.LoadAssetsAsync(taskConfig);

        result.Should().NotBeNull();
        result.PromptTemplate.Should().Be(promptContent);
    }

    [Test]
    public void Given_ValidTaskConfig_When_LoadAssetsAsyncIsCalled_Then_CallsPromptLoaderWithCorrectPath()
    {
        var builder = new Builder();
        var promptContent = builder.CreatePromptContent();
        var promptPath = builder.CreatePromptPath();
        builder.WithPromptContent(promptContent);
        var sut = builder.BuildSut();
        var taskConfig = builder.CreateTaskConfig(promptPath);

        sut.LoadAssetsAsync(taskConfig);

        builder.PromptLoaderMock.Verify(x => x.LoadPrompt(promptPath), Times.Once);
    }

    [Test]
    public void Given_PromptLoaderReturnsEmptyString_When_LoadAssetsAsyncIsCalled_Then_ReturnsLoadedTaskAssetsWithEmptyTemplate()
    {
        var builder = new Builder().WithPromptContent(string.Empty);
        var sut = builder.BuildSut();
        var taskConfig = builder.CreateTaskConfig();

        var result = sut.LoadAssetsAsync(taskConfig);

        result.Should().NotBeNull();
        result.PromptTemplate.Should().BeEmpty();
    }

    [Test]
    public void Given_DifferentPromptPath_When_LoadAssetsAsyncIsCalled_Then_CallsPromptLoaderWithProvidedPath()
    {
        var builder = new Builder();
        var promptContent = builder.CreatePromptContent();
        var customPath = builder.CreatePromptPath();
        builder.WithPromptContent(promptContent);
        var sut = builder.BuildSut();
        var taskConfig = builder.CreateTaskConfig(customPath);

        sut.LoadAssetsAsync(taskConfig);

        builder.PromptLoaderMock.Verify(x => x.LoadPrompt(customPath), Times.Once);
    }
}
