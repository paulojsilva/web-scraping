﻿using AngleSharp.Html.Parser;
using Domain.Services.Interfaces;
using Domain.Shared.Configuration;
using Domain.Shared.Dto;
using Flunt.Notifications;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Domain.Services.Implementations
{
    public abstract class ScraperService : Notifiable, IScraper
    {
        protected readonly IHttpClientFactory httpClientFactory;
        protected readonly ICache cache;
        protected readonly IOptions<AppSettings> settings;
        protected HttpClient httpClient;
        protected string host;
        protected HtmlParser documentParser = new HtmlParser();

        public ScraperService(IHttpClientFactory httpClientFactory, ICache cache, IOptions<AppSettings> settings)
        {
            this.httpClientFactory = httpClientFactory;
            this.cache = cache;
            this.settings = settings;
        }

        public virtual async Task<ScraperDataResponse> GetAsync(ScraperRequest request)
        {
            var response = new ScraperDataResponse();

            try
            {
                this.httpClient = CreateHttpClient();

                var httpRequest = await httpClient.GetAsync(request.Url);

                if (!httpRequest.IsSuccessStatusCode)
                    AddNotification("HttpClient", "HTTP response was unsuccessful.");

                response.Data = await httpRequest.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                AddNotification(ex.GetType().Name, ex.GetMessageConcatenatedWithInner());
            }

            return response;
        }

        public abstract Task<List<GroupingFileInformationResponse>> GetGroupingFileInformationAsync(ScraperRequest request);

        public Notifiable GetNotifiable() => this;

        protected HttpClient CreateHttpClient() => this.httpClientFactory.CreateClient();
    }
}