using AngleSharp;
using Domain.Dom.GitHub;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class TestGitHubParser
    {
        protected readonly IBrowsingContext context;

        public TestGitHubParser()
        {
            context = BrowsingContext.New(Configuration.Default);
        }

        [Fact(DisplayName = "Last Commit Hash - found")]
        public async Task ShouldGetLastCommitHash()
        {
            // Given
            var source = @"
            <div class=""Box-header Box-header--blue position-relative"">
                <h2 class=""sr-only"">Latest commit</h2>
                <div class=""js-details-container Details d-flex rounded-top-1 flex-items-center flex-wrap"" data-issue-and-pr-hovercards-enabled="""">
              <div class=""flex-1 d-flex flex-items-center ml-3 min-width-0"">
                <div class=""css-truncate css-truncate-overflow text-gray"">
                    <span class=""commit-author user-mention"">Paulo Justino</span>
                    <span class=""d-none d-sm-inline"">
                      <a data-pjax=""true"" title=""Documentation"" class=""link-gray-dark"" href=""/paulojsilva/web-scraping-nolayer/commit/1cf47cd55b5c745bb8dfb734c8f96614c6b30273"">Documentation</a>
                    </span>
                </div>
                <span class=""hidden-text-expander ml-2 d-inline-block d-inline-block d-lg-none"">
                  <button type=""button"" class=""hx_bg-black-fade-15 text-gray-dark ellipsis-expander js-details-target"" aria-expanded=""false"">…</button>
                </span>
                <div class=""d-flex flex-auto flex-justify-end ml-3 flex-items-baseline"">
        
                  <a href=""/paulojsilva/web-scraping-nolayer/commit/1cf47cd55b5c745bb8dfb734c8f96614c6b30273"" class=""f6 link-gray text-mono ml-2 d-none d-lg-inline"" data-pjax="""">1cf47cd</a>
                  <a href=""/paulojsilva/web-scraping-nolayer/commit/1cf47cd55b5c745bb8dfb734c8f96614c6b30273"" class=""link-gray ml-2"" data-pjax="""">
                    <relative-time datetime=""2021-02-02T14:54:52Z"" class=""no-wrap"" title=""2 de fev. de 2021 11:54 BRT"">12 hours ago</relative-time>
                  </a>
                </div>
              </div>
                </div>
              </div>";

            var document = await context.OpenAsync(req => req.Content(source));
            var parser = new GitHubParser(document, "github.com");

            // When
            var lastCommitHash = parser.GetLastCommitHash();

            // Then
            lastCommitHash.Should().Be("1cf47cd55b5c745bb8dfb734c8f96614c6b30273");
        }

        [Fact(DisplayName = "Page type - discovered")]
        public async Task ShouldDiscoverPageType()
        {
            // Given
            var source = @"
            <div itemprop=""text"" class=""Box-body p-0 blob-wrapper data type-c  gist-border-0"">
            <table class=""highlight tab-size js-file-line-container"" data-tab-size=""8"" data-paste-markdown-skip=""""></table>
            <details class=""details-reset details-overlay BlobToolbar position-absolute js-file-line-actions dropdown d-none"" aria-hidden=""true"">
            </details>
            </div>";

            var document = await context.OpenAsync(req => req.Content(source));
            var parser = new GitHubParser(document, "github.com");

            // When
            var pageType = parser.DiscoverPageType();

            // Then
            pageType.Should().Be(GitHubParser.GitHubPageType.FileContent);
        }

        [Fact(DisplayName = "File informations (lines and size) - found")]
        public async Task ShouldGetFileInformation()
        {
            // Given
            var source = @"
            <h2 id=""blob-path"" class=""breadcrumb flex-auto min-width-0 text-normal mx-0 mx-md-3 width-full width-md-auto flex-order-1 flex-md-order-none mt-3 mt-md-0"">
            <span class=""js-repo-root text-bold""><span class=""js-path-segment d-inline-block wb-break-all""><a data-pjax=""true"" href=""/paulojsilva/web-scraping-nolayer""><span>web-scraping-nolayer</span></a></span></span><span class=""separator"">/</span><strong class=""final-path"">Startup.cs</strong>
            </h2>
            <div class=""Box-header py-2 d-flex flex-column flex-shrink-0 flex-md-row flex-md-items-center"">
              <div class=""text-mono f6 flex-auto pr-3 flex-order-2 flex-md-order-1 mt-2 mt-md-0"">

                  76 lines (63 sloc)
                  <span class=""file-info-divider""></span>
                2.33 KB
              </div>
            </div>";

            var document = await context.OpenAsync(req => req.Content(source));
            var parser = new GitHubParser(document, "github.com");

            // When
            var fileInformation = parser.GetFileInformation();

            // Then
            fileInformation.Bytes.Should().Be(2330);
            fileInformation.Lines.Should().Be(76);
        }

        [Fact(DisplayName = "File name - found")]
        public async Task ShouldGetFileNameOnFileContent()
        {
            // Given
            var source = @"
            <h2 id=""blob-path"" class=""breadcrumb flex-auto min-width-0 text-normal mx-0 mx-md-3 width-full width-md-auto flex-order-1 flex-md-order-none mt-3 mt-md-0"">
            <span class=""js-repo-root text-bold""><span class=""js-path-segment d-inline-block wb-break-all""><a data-pjax=""true"" href=""/paulojsilva/web-scraping-nolayer""><span>web-scraping-nolayer</span></a></span></span><span class=""separator"">/</span><strong class=""final-path"">Startup.cs</strong>
            </h2>";

            var document = await context.OpenAsync(req => req.Content(source));
            var parser = new GitHubParser(document, "github.com");

            // When
            var fileName = parser.GetFileNameOnFileContent();

            // Then
            fileName.Should().Be("Startup.cs");
        }

        [Fact(DisplayName = "Folder list itens - found")]
        public async Task ShouldGetFolderListItens()
        {
            // Given
            var source = @"
            <div class=""js-details-container Details"">
            <div role=""grid"" aria-labelledby=""files"" class=""Details-content--hidden-not-important js-navigation-container js-active-navigation-container d-block"" data-pjax="""">
                <div role=""row"" class=""Box-row Box-row--focus-gray py-2 d-flex position-relative js-navigation-item"">
                  <div role=""gridcell"" class=""mr-3 flex-shrink-0"" style=""width: 16px;"">
                      <svg aria-label=""Directory"" class=""octicon octicon-file-directory text-color-icon-directory"" height=""16"" viewBox=""0 0 16 16"" version=""1.1"" width=""16"" role=""img""><path fill-rule=""evenodd"" d=""M1.75 1A1.75 1.75 0 000 2.75v10.5C0 14.216.784 15 1.75 15h12.5A1.75 1.75 0 0016 13.25v-8.5A1.75 1.75 0 0014.25 3h-6.5a.25.25 0 01-.2-.1l-.9-1.2c-.33-.44-.85-.7-1.4-.7h-3.5z""></path></svg>
                  </div>
                  <div role=""rowheader"" class=""flex-auto min-width-0 col-md-2 mr-3"">
                    <span class=""css-truncate css-truncate-target d-block width-fit""><a class=""js-navigation-open link-gray-dark"" title=""This path skips through empty directories"" data-pjax=""#repo-content-pjax-container"" href=""/paulojsilva/web-scraping-nolayer/tree/main/Layers/Domain/Dom/GitHub""><span class=""text-gray-light"">Dom/</span>GitHub</a></span>
                  </div>
                  <div role=""gridcell"" class=""flex-auto min-width-0 d-none d-md-block col-5 mr-3"">
                      <span class=""css-truncate css-truncate-target d-block width-fit"">
                            <a data-pjax=""true"" title=""first commit"" class=""link-gray"" href=""/paulojsilva/web-scraping-nolayer/commit/c95e6cb33ab2d712c4cc93767808061ee9469f3a"">first commit</a>
                      </span>
                  </div>
                  <div role=""gridcell"" class=""text-gray-light text-right"" style=""width:100px;"">
                      <time-ago datetime=""2021-02-02T13:14:42Z"" class=""no-wrap"" title=""2 de fev. de 2021 10:14 BRT"">14 hours ago</time-ago>
                  </div>
                </div>
                <div role=""row"" class=""Box-row Box-row--focus-gray py-2 d-flex position-relative js-navigation-item navigation-focus"">
                  <div role=""gridcell"" class=""mr-3 flex-shrink-0"" style=""width: 16px;"">
                      <svg aria-label=""Directory"" class=""octicon octicon-file-directory text-color-icon-directory"" height=""16"" viewBox=""0 0 16 16"" version=""1.1"" width=""16"" role=""img""><path fill-rule=""evenodd"" d=""M1.75 1A1.75 1.75 0 000 2.75v10.5C0 14.216.784 15 1.75 15h12.5A1.75 1.75 0 0016 13.25v-8.5A1.75 1.75 0 0014.25 3h-6.5a.25.25 0 01-.2-.1l-.9-1.2c-.33-.44-.85-.7-1.4-.7h-3.5z""></path></svg>
                  </div>
                  <div role=""rowheader"" class=""flex-auto min-width-0 col-md-2 mr-3"">
                    <span class=""css-truncate css-truncate-target d-block width-fit""><a class=""js-navigation-open link-gray-dark"" title=""Services"" data-pjax=""#repo-content-pjax-container"" href=""/paulojsilva/web-scraping-nolayer/tree/main/Layers/Domain/Services"">Services</a></span>
                  </div>
                  <div role=""gridcell"" class=""flex-auto min-width-0 d-none d-md-block col-5 mr-3"">
                      <span class=""css-truncate css-truncate-target d-block width-fit"">
                            <a data-pjax=""true"" title=""first commit"" class=""link-gray"" href=""/paulojsilva/web-scraping-nolayer/commit/c95e6cb33ab2d712c4cc93767808061ee9469f3a"">first commit</a>
                      </span>
                  </div>
                  <div role=""gridcell"" class=""text-gray-light text-right"" style=""width:100px;"">
                      <time-ago datetime=""2021-02-02T13:14:42Z"" class=""no-wrap"" title=""2 de fev. de 2021 10:14 BRT"">14 hours ago</time-ago>
                  </div>
                </div>
            </div>
          </div>";

            var document = await context.OpenAsync(req => req.Content(source));
            var parser = new GitHubParser(document, "github.com");

            // When
            var itens = parser.GetFolderListItens();

            // Then
            itens.Should().HaveCount(2);
            itens.First(i => i.Type == GitHubLinkAccess.GitHubLinkAccessType.Folder && i.Endpoint.EndsWith("Dom/GitHub")).Should().NotBeNull();
            itens.Last(i => i.Type == GitHubLinkAccess.GitHubLinkAccessType.Folder && i.Endpoint.EndsWith("Services")).Should().NotBeNull();
        }
    }
}
