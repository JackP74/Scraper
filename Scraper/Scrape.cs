using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    class Scrape
    {
        private string currentURL;
        private OffScreenBrowser browser;

        public Scrape()
        {
            browser = new OffScreenBrowser();
        }

    }
}
