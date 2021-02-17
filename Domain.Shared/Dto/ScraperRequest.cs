using System;

namespace Domain.Shared.Dto
{
    public class ScraperRequest
    {
        private const string http = "http://";
        private const string https = "https://";

        public ScraperRequest(string url)
        {
            this.Url = url?.ToLowerInvariant();
             
            if (!string.IsNullOrWhiteSpace(Url))
            {
                this.PrefixHttp();
                this.SetAuthority();
            }
        }

        private void PrefixHttp()
        {
            if (this.Url.StartsWith(http))
            {
                this.Url = this.Url.Replace(http, https);
            }
            else if (!this.Url.StartsWith(https))
            {
                this.Url = https + this.Url;
            }

            this.Host = https;
        }

        private void SetAuthority()
        {
            try
            {
                this.Host += new Uri(Url).Authority;
            }
            catch
            {
                throw new Exception($"Error when instance new Uri from ({this.Url}) and get Authority");
            }
        }

        public string Host { get; protected set; }

        public string Url { get; protected set; }
    }
}
