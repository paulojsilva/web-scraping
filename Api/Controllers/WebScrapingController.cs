using Application.Common;
using Application.Services.Interfaces;
using Domain.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WebScrapingController : ControllerBase
    {
        private readonly IAppScraper scraper;

        public WebScrapingController(IAppScraper scraper)
        {
            this.scraper = scraper;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> RunAsync([FromQuery] string url)
        {
            try
            {
                var request = new ScraperRequest(url);
                var response = await this.scraper.GetGroupingFileInformationAsync(request);
                return HandleResult(response);
            }
            catch (Exception ex)
            {
                return HandleResult(ServiceResult.Error(ex));
            }
        }

        protected virtual IActionResult HandleResult(ServiceResult serviceResult)
        {
            if (serviceResult.Status == ServiceResult.StatusResult.Error)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
         
            return new JsonResult(serviceResult);
        }
    }
}
