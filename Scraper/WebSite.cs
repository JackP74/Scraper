using System;
using System.Collections.Generic;

namespace Scraper
{
    [Serializable]
    public class WebSiteStorage
    {
        public List<WebSite> URLs = new List<WebSite>();

        public WebSiteStorage()
        {
        }
    }

    [Serializable]
    public class WebSite
    {
        public string URL { get; set; }

        public bool Active { get; set; }

        public string Status { get; set; }

        public WebSite()
        {
        }

        public WebSite(string newURL, bool newActive, string newStatus)
        {
            this.URL = newURL;
            this.Active = newActive;
            this.Status = newStatus;
        }
    }
}
