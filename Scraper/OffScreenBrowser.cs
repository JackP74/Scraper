using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper
{
    class OffScreenBrowser
    {
        private readonly ChromiumWebBrowser browser;
        private bool waitForBrowser = false;
        private bool scriptNeeded = false;
        private string scriptToRun = string.Empty;

        public OffScreenBrowser()
        {
            browser = new ChromiumWebBrowser("about:blank");
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
        }

        public string Get(string uri, bool scriptNeeded = false, string scriptToRun = "")
        {
            waitForBrowser = true;

            this.scriptNeeded = scriptNeeded;
            this.scriptToRun = scriptToRun;

            browser.Load(uri);

            while (waitForBrowser)
                Thread.Sleep(200);

            return browser.GetSourceAsync().Result;
        }

        public void DownloadImages(string url)
        {
            string host = new Uri(url).Host;
            if (host.StartsWith("www.")) host = host.Substring(4);

            switch(host)
            {
                case "topescortbabes.com":
                    {
                        waitForBrowser = true;

                        browser.Load(url);

                        while (waitForBrowser)
                            Thread.Sleep(200);



                        break;
                    }
                default:
                    break;
            }
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                string url = e.Browser.MainFrame.Url;
                string host = new Uri(url).Host;

                if (host.StartsWith("www."))
                    host = host.Substring(4);

                if (Globals.AcceptedDomains.Contains(host))
                {
                    if (scriptNeeded)
                    {
                        var scriptTask = browser.EvaluateScriptAsync(scriptToRun);

                        scriptTask.ContinueWith(t =>
                        {
                            scriptNeeded = false;
                            scriptToRun = string.Empty;
                            waitForBrowser = false;
                        }, TaskScheduler.Default);
                    }
                    else
                    {
                        scriptToRun = string.Empty;
                        waitForBrowser = false;
                    }
                }
            }
        }
    }
}
