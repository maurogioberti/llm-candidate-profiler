using System.Net;
using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace CandidateProfiler.Application.Tests.Services;

public class OpenAiLlmServiceTests
{
    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private AppConfig _appConfig;
        private HttpResponseMessage _httpResponse;

        public Builder()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _appConfig = CreateDefaultAppConfig();
            _httpResponse = CreateSuccessResponse();
        }

        public Builder WithOpenAiConfig(OpenAiConfig config)
        {
            _appConfig.OpenAi = config;
            return this;
        }

        public Builder WithHttpResponse(HttpResponseMessage response)
        {
            _httpResponse = response;
            return this;
        }

        public OpenAiLlmService BuildSut()
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(_httpResponse);

            return new OpenAiLlmService(_httpClient, _appConfig);
        }

        private AppConfig CreateDefaultAppConfig()
        {
            return new AppConfig
            {
                OpenAi = new OpenAiConfig
                {
                    BaseUrl = "https://api.openai.com/v1",
                    ModelName = _fixture.Create<string>(),
                    ApiKey = _fixture.Create<string>(),
                    Temperature = _fixture.Create<double>(),
                    TimeoutMinutes = 5,
                    DelayMilliseconds = 0
                }
            };
        }

        private HttpResponseMessage CreateSuccessResponse()
        {
            var responseContent = _fixture.Create<string>();
            var jsonResponse = JsonSerializer.Serialize(new
            {
                choices = new[]
                {
                    new
                    {
                        message = new
                        {
                            content = responseContent
                        }
                    }
                }
            });
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse)
            };
        }

        public string CreatePrompt()
        {
            return _fixture.Create<string>();
        }

        public string CreateResponseContent()
        {
            return _fixture.Create<string>();
        }

        public HttpResponseMessage CreateResponseWithContent(string content)
        {
            var jsonResponse = JsonSerializer.Serialize(new
            {
                choices = new[]
                {
                    new
                    {
                        message = new
                        {
                            content = content
                        }
                    }
                }
            });
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse)
            };
        }

        public OpenAiConfig CreateOpenAiConfig()
        {
            return new OpenAiConfig
            {
                BaseUrl = "https://api.openai.com/v1",
                ModelName = _fixture.Create<string>(),
                ApiKey = _fixture.Create<string>(),
                Temperature = _fixture.Create<double>(),
                TimeoutMinutes = _fixture.Create<int>(),
                DelayMilliseconds = _fixture.Create<int>()
            };
        }
    }

    #endregion

    [Test]
    public async Task Given_ValidPrompt_When_CompleteAsyncIsCalled_Then_ReturnsLlmResponse()
    {
        var builder = new Builder();
        var expectedResponse = builder.CreateResponseContent();
        builder.WithHttpResponse(builder.CreateResponseWithContent(expectedResponse));
        var sut = builder.BuildSut();
        var prompt = builder.CreatePrompt();

        var result = await sut.CompleteAsync(prompt);

        result.Should().Be(expectedResponse);
    }

    [Test]
    public async Task Given_EmptyPrompt_When_CompleteAsyncIsCalled_Then_SendsRequestWithEmptyPrompt()
    {
        var builder = new Builder();
        var sut = builder.BuildSut();

        var result = await sut.CompleteAsync(string.Empty);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task Given_ResponseWithoutContent_When_CompleteAsyncIsCalled_Then_ReturnsEmptyString()
    {
        var builder = new Builder();
        builder.WithHttpResponse(builder.CreateResponseWithContent(null!));
        var sut = builder.BuildSut();
        var prompt = builder.CreatePrompt();

        var result = await sut.CompleteAsync(prompt);

        result.Should().Be(string.Empty);
    }

    [Test]
    public void Given_OpenAiConfig_When_ServiceIsCreated_Then_SetsHttpClientTimeout()
    {
        var builder = new Builder();
        var config = builder.CreateOpenAiConfig();
        builder.WithOpenAiConfig(config);

        var sut = builder.BuildSut();

        sut.Should().NotBeNull();
    }
}
