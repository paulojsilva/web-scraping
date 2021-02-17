using Domain.Services.Implementations;
using Domain.Services.Interfaces;
using Domain.Shared.Configuration;
using Domain.Shared.Dto;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class TestGitHubScraperService
    {
        [Fact(DisplayName = "Grouping File Information - works")]
        public async Task ShouldGetGroupingFileInformation()
        {
            // Given

            var folderListUrl = "https://github.com/paulojsilva/web-scraping/tree/main/domain.shared/configuration";
            var mocksDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\Mocks\\";

            var streamFolderList = new FileStream($"{mocksDirectory}\\github-FolderList.html", FileMode.Open, FileAccess.Read);
            var streamAppSettings = new FileStream($"{mocksDirectory}\\github-AppSettings.html", FileMode.Open, FileAccess.Read);
            var streamAuthenticationSettings = new FileStream($"{mocksDirectory}\\github-AuthenticationSettings.html", FileMode.Open, FileAccess.Read);
            var streamCacheSettings = new FileStream($"{mocksDirectory}\\github-CacheSettings.html", FileMode.Open, FileAccess.Read);
            var streamParallelismSettings = new FileStream($"{mocksDirectory}\\github-ParallelismSettings.html", FileMode.Open, FileAccess.Read);

            var streamContentFolderList = new StreamContent(streamFolderList);
            var streamContentAppSettings = new StreamContent(streamAppSettings);
            var streamContentAuthenticationSettings = new StreamContent(streamAuthenticationSettings);
            var streamContentCacheSettings = new StreamContent(streamCacheSettings);
            var streamContentParallelismSettings = new StreamContent(streamParallelismSettings);
            
            var mockHttpMessageHandler = new MockHttpMessageHandler();

            mockHttpMessageHandler
                .When(folderListUrl)
                .Respond(response => new HttpResponseMessage(HttpStatusCode.OK) { Content = streamContentFolderList });

            mockHttpMessageHandler
                .When("*AppSettings*")
                .Respond(response => new HttpResponseMessage(HttpStatusCode.OK) { Content = streamContentAppSettings });

            mockHttpMessageHandler
                .When("*AuthenticationSettings*")
                .Respond(response => new HttpResponseMessage(HttpStatusCode.OK) { Content = streamContentAuthenticationSettings });

            mockHttpMessageHandler
                .When("*CacheSettings*")
                .Respond(response => new HttpResponseMessage(HttpStatusCode.OK) { Content = streamContentCacheSettings });

            mockHttpMessageHandler
                .When("*ParallelismSettings*")
                .Respond(response => new HttpResponseMessage(HttpStatusCode.OK) { Content = streamContentParallelismSettings });

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            mockHttpClientFactory
                .Setup(s => s.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(mockHttpMessageHandler));

            var mockCache = new Mock<ICache>();

            mockCache
                .Setup(s => s.Get<It.IsAnyType>(It.IsAny<string>()))
                .Returns(default(It.IsAnyType));

            var mockParallelismSettings = Options.Create(new ParallelismSettings { IncreaseDelayGoSlowly = 2, MaxDegreeOfParallelism = 10, MaxHttpRequestInParallel = 2 });
            var service = new GitHubScraperService(mockHttpClientFactory.Object, mockCache.Object, mockParallelismSettings);
            var request = new ScraperRequest(folderListUrl);

            // When
            var response = await service.GetGroupingFileInformationAsync(request);

            // Then
            response.Should().HaveCount(1);
            response.First().Details.Should().HaveCount(4);
            response.First().TotalNumberFiles.Should().Be(4);
            response.First().TotalNumberBytes.Should().Be(1447);
            response.First().TotalNumberLines.Should().Be(60);
            response.First().Extension.Should().Be(".cs");
        }
    }
}
