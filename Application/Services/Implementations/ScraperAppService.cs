using Application.Common;
using Application.Services.Interfaces;
using Domain.Services.Interfaces;
using Domain.Shared.Dto;
using Flunt.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public abstract class ScraperAppService : Notifiable, IAppScraper
    {
        protected readonly IScraper domainService;

        public ScraperAppService(IScraper domainService)
        {
            this.domainService = domainService;
        }

        public virtual async Task<ServiceTResult<ScraperDataResponse>> GetAsync(ScraperRequest request)
        {
            this.FastValidation(request);

            if (Invalid) return this.HandleResult<ScraperDataResponse>(null);

            var data = await this.domainService.GetAsync(request);
            
            return this.HandleResult(data);
        }

        public abstract Task<ServiceTResult<List<GroupingFileInformationResponse>>> GetGroupingFileInformationAsync(ScraperRequest request);

        protected virtual void FastValidation(ScraperRequest request)
        {
            if (request == null)
            {
                AddNotification("ScraperRequest", "Is null");
                return;
            }

            if (string.IsNullOrWhiteSpace(request.Url))
            {
                AddNotification("ScraperRequest.Url", "Is null/empty");
            }
        }

        protected virtual ServiceTResult<T> HandleResult<T>(T data)
        {
            AddNotifications(this.domainService.GetNotifiable());

            return new ServiceTResult<T>
            {
                Data = data,
                Notifications = base.Notifications,
                Status = Invalid ? ServiceResult.StatusResult.Error : ServiceResult.StatusResult.Ok
            };
        }

        protected virtual ServiceResult HandleResult()
        {
            AddNotifications(this.domainService.GetNotifiable());

            return new ServiceResult
            {
                Notifications = base.Notifications,
                Status = Invalid ? ServiceResult.StatusResult.Error : ServiceResult.StatusResult.Ok
            };
        }
    }
}
