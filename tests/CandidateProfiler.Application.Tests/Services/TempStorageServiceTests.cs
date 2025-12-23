using AutoFixture;
using AutoFixture.AutoMoq;
using CandidateProfiler.Application.Domain.Config;
using CandidateProfiler.Application.Services;
using FluentAssertions;

namespace CandidateProfiler.Application.Tests.Services;

public class TempStorageServiceTests
{
    private const int InitialPageNumber = 0;
    private const string SpecialCharsDocId = "doc:id/with\\special*chars?.pdf";
    private string _testTempFolder;

    #region Builder

    private class Builder
    {
        private readonly IFixture _fixture;
        private string _tempFolder;

        public Builder(string tempFolder)
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _tempFolder = tempFolder;
        }

        public Builder WithTempFolder(string folder)
        {
            _tempFolder = folder;
            return this;
        }

        public TempStorageService BuildSut()
        {
            var appConfig = CreateAppConfig();
            return new TempStorageService(appConfig);
        }

        private AppConfig CreateAppConfig()
        {
            return new AppConfig
            {
                Paths = new PathsConfig
                {
                    Temp = _tempFolder
                }
            };
        }

        public List<string> CreateProcessedTexts()
        {
            return _fixture.CreateMany<string>(3).ToList();
        }

        public string CreateDocId()
        {
            return _fixture.Create<string>();
        }

        public int CreatePageNumber()
        {
            return _fixture.Create<int>();
        }
    }

    #endregion

    [SetUp]
    public void SetUp()
    {
        _testTempFolder = $"TestTemp_{Guid.NewGuid():N}";
    }

    [Test]
    public async Task Given_NewDocId_When_SaveProgressAsyncIsCalled_Then_CreatesProgressFile()
    {
        var builder = new Builder(_testTempFolder);
        var sut = builder.BuildSut();
        var docId = builder.CreateDocId();
        var pageNumber = builder.CreatePageNumber();
        var processedTexts = builder.CreateProcessedTexts();

        await sut.SaveProgressAsync(docId, pageNumber, processedTexts);

        var (actualPageNumber, actualTexts) = await sut.LoadProgressAsync(docId);
        actualPageNumber.Should().Be(pageNumber);
        actualTexts.Should().BeEquivalentTo(processedTexts);
    }

    [Test]
    public async Task Given_NonExistentDocId_When_LoadProgressAsyncIsCalled_Then_ReturnsDefaultValues()
    {
        var builder = new Builder(_testTempFolder);
        var sut = builder.BuildSut();
        var nonExistentDocId = builder.CreateDocId();

        var (pageNumber, texts) = await sut.LoadProgressAsync(nonExistentDocId);

        pageNumber.Should().Be(InitialPageNumber);
        texts.Should().BeEmpty();
    }

    [Test]
    public async Task Given_ExistingProgress_When_SaveProgressAsyncIsCalledAgain_Then_OverwritesProgress()
    {
        var builder = new Builder(_testTempFolder);
        var sut = builder.BuildSut();
        var docId = builder.CreateDocId();
        var initialPageNumber = builder.CreatePageNumber();
        var updatedPageNumber = builder.CreatePageNumber();
        var initialTexts = builder.CreateProcessedTexts();
        var updatedTexts = builder.CreateProcessedTexts();

        await sut.SaveProgressAsync(docId, initialPageNumber, initialTexts);
        await sut.SaveProgressAsync(docId, updatedPageNumber, updatedTexts);

        var (pageNumber, texts) = await sut.LoadProgressAsync(docId);
        pageNumber.Should().Be(updatedPageNumber);
        texts.Should().BeEquivalentTo(updatedTexts);
    }

    [Test]
    public async Task Given_DocIdWithSpecialCharacters_When_SaveProgressAsyncIsCalled_Then_SanitizesFileName()
    {
        var builder = new Builder(_testTempFolder);
        var sut = builder.BuildSut();
        var pageNumber = builder.CreatePageNumber();
        var processedTexts = builder.CreateProcessedTexts();

        await sut.SaveProgressAsync(SpecialCharsDocId, pageNumber, processedTexts);

        var (actualPageNumber, actualTexts) = await sut.LoadProgressAsync(SpecialCharsDocId);
        actualPageNumber.Should().Be(pageNumber);
        actualTexts.Should().BeEquivalentTo(processedTexts);
    }

    [Test]
    public async Task Given_EmptyProcessedTexts_When_SaveProgressAsyncIsCalled_Then_SavesEmptyList()
    {
        var builder = new Builder(_testTempFolder);
        var sut = builder.BuildSut();
        var docId = builder.CreateDocId();
        var pageNumber = builder.CreatePageNumber();
        var emptyTexts = new List<string>();

        await sut.SaveProgressAsync(docId, pageNumber, emptyTexts);

        var (actualPageNumber, actualTexts) = await sut.LoadProgressAsync(docId);
        actualPageNumber.Should().Be(pageNumber);
        actualTexts.Should().BeEmpty();
    }

    [Test]
    public async Task Given_ZeroPageNumber_When_SaveProgressAsyncIsCalled_Then_SavesZeroPageNumber()
    {
        var builder = new Builder(_testTempFolder);
        var sut = builder.BuildSut();
        var docId = builder.CreateDocId();
        var processedTexts = builder.CreateProcessedTexts();

        await sut.SaveProgressAsync(docId, InitialPageNumber, processedTexts);

        var (pageNumber, texts) = await sut.LoadProgressAsync(docId);
        pageNumber.Should().Be(InitialPageNumber);
        texts.Should().BeEquivalentTo(processedTexts);
    }

    [Test]
    public void Given_AppConfigWithTempPath_When_ServiceIsCreated_Then_CreatesTempDirectory()
    {
        var newTempFolder = $"NewTestTemp_{Guid.NewGuid():N}";
        var builder = new Builder(newTempFolder);

        var sut = builder.BuildSut();

        Directory.Exists(newTempFolder).Should().BeTrue();
        Directory.Delete(newTempFolder);
    }

    [TearDown]
    public void Cleanup()
    {
        if (Directory.Exists(_testTempFolder))
        {
            Directory.Delete(_testTempFolder, recursive: true);
        }
    }
}
