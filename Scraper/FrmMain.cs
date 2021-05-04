using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using HtmlAgilityPack;
using MessageCustomHandler;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Scraper
{
    //TODO: add pages to scrapes
    public partial class FrmMain : Form
    {
        #region "Variables"
        private readonly string newLine = Environment.NewLine;
        private readonly string AppPath = Application.StartupPath;
        private readonly string URLsPath = Path.Combine(Application.StartupPath, "urls.xml");

        private WebSiteStorage Sites = new WebSiteStorage();
        private readonly XmlSerializer xmlserializer = new XmlSerializer(typeof(WebSiteStorage));

        private OffScreenBrowser browser = new OffScreenBrowser();
        private bool Scraping = false;
        private Thread threadScrape;
        #endregion

        #region "Win32 Imports"
        [DllImport("kernel32.dll")]
        public static extern Int32 AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

        public const int STD_OUTPUT_HANDLE = -11;
        public const int STD_INPUT_HANDLE = -10;
        public const int STD_ERROR_HANDLE = -12;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename,
                                               [MarshalAs(UnmanagedType.U4)] uint access,
                                               [MarshalAs(UnmanagedType.U4)] FileShare share,
                                                                                 IntPtr securityAttributes,
                                               [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                                               [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
                                                                                 IntPtr templateFile);

        public const uint GENERIC_WRITE = 0x40000000;
        public const uint GENERIC_READ = 0x80000000;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private const int EM_SETCUEBANNER = 0x1501;

        private static void OverrideRedirection()
        {
            var hOut = GetStdHandle(STD_OUTPUT_HANDLE);
            var hRealOut = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE, FileShare.Write, IntPtr.Zero, FileMode.OpenOrCreate, 0, IntPtr.Zero);
            if (hRealOut != hOut)
            {
                SetStdHandle(STD_OUTPUT_HANDLE, hRealOut);
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding) { AutoFlush = true });
            }
        }
        #endregion

        #region "Functions"
        public FrmMain()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            InitializeComponent();
            CreateConsole();
        }

        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }

        private void StartScape()
        {
            if (Scraping)
            {
                LogWarning("Already started");

                BtnStart.Text = "Stop";
                LabelStatus.Text = "Getting profiles...";

                return;
            }
            else
                Scraping = true;

            if (threadScrape != null)
            {
                try
                {
                    threadScrape.Abort();
                    threadScrape = null;
                }
                catch { }
            }

            Log("Starting...");

            threadScrape = new Thread(() =>
            {
                foreach (var site in Sites.URLs.Where(x => x.Active))
                {
                    string url = site.URL;
                    string host = new Uri(url).Host;

                    if (host.StartsWith("www."))
                        host = host.Substring(4);

                    if (Globals.AcceptedDomains.Contains(host))
                    {
                        switch (host)
                        {
                            case "eurogirlsescort.com":
                                {
                                    Log($"Getting user profiles from '{url}'");
                                    //ScrapeEuroGirlsEscort(url);
                                    break;
                                }
                            case "topescortbabes.com":
                                {
                                    Log($"Getting user profiles from '{url}'");
                                    ScrapeTopEscortBabes(url);
                                    break;
                                }
                            default:
                                LogWarning($"Domain {host} unsupported and skipped.");
                                continue;
                        }
                    }
                    else
                    {
                        LogWarning($"Domain {host} unsupported and skipped.");
                        continue;
                    }
                }

                Log(newLine + "Done" + newLine);
                StopScrape();
            }) { IsBackground = true };

            threadScrape.SetApartmentState(ApartmentState.STA);
            threadScrape.Start();
        }

        private void StopScrape(bool endMessage = false)
        {
            if (!Scraping)
            {
                LogWarning("Already stopped");

                BtnStart.Text = "Start";
                LabelStatus.Text = "Idle...";

                return;
            }
            else
            {
                if (threadScrape != null)
                {
                    try
                    {
                        threadScrape.Abort();
                        threadScrape = null;
                    }
                    catch { }
                }
            }

            Scraping = false;

            if (endMessage)
                Log("Stopped by user");
        }

        public void CreateConsole()
        {
            AllocConsole();

            OverrideRedirection();

            Console.Title = "Scraper Console";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Scraper v0.1 alpha");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
        }

        public void Log(string txt)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(txt);
        }

        public void LogWarning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(warning);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LogError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void SetCueText(Control control, string text)
        {
            SendMessage(control.Handle, EM_SETCUEBANNER, 0, text);
        }

        private void StartThread(ThreadStart newStart)
        {
            Thread newThread = new Thread(newStart) { IsBackground = true };
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
        }

        private void LoadURLs()
        {
            try
            {
                if (File.Exists(URLsPath))
                {
                    using Stream fStream = new FileStream(URLsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    using XmlReader xStream = XmlReader.Create(fStream);

                    Sites = (WebSiteStorage)xmlserializer.Deserialize(xStream);

                    RefreshUrlList();
                }
                else
                {
                    SaveURLs();
                }
            }
            catch (Exception ex)
            {
                LogError("Unable to read URLs. URLs backup and reset. Error: " + ex.Message);

                if (File.Exists(URLsPath))
                {
                    if (File.Exists($"{URLsPath}.bk"))
                    {
                        int i = 0;
                        while (true)
                        {
                            i++;
                            if (!File.Exists($"{URLsPath}.bk{i}"))
                            {
                                File.Move(URLsPath, $"{URLsPath}.bk{i}");
                                break;
                            }
                        }
                    }
                    else
                    {
                        File.Move(URLsPath, $"{URLsPath}.bk");
                    }
                }

                Sites.URLs.Clear();
                SaveURLs();
            }
        }

        private void SaveURLs()
        {
            try
            {
                if (File.Exists(URLsPath))
                    File.Delete(URLsPath);

                using Stream fStream = new FileStream(URLsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                using XmlWriter xStream = XmlWriter.Create(fStream);

                xmlserializer.Serialize(xStream, Sites);
            }
            catch (Exception ex)
            {
                LogError("Unable to save URLs. Error: " + ex.Message);
            }
        }

        private void RefreshUrlList()
        {
            this.Invoke(new Action(() => {

                ListURLs.Items.Clear();

                foreach (WebSite url in Sites.URLs)
                    ListURLs.Items.Add(url.URL).SubItems.AddRange(new[] { url.Active.ToString(), url.Status });
            
            }));
        }
        #endregion

        #region "Scrape Functions"
        private bool ScrapeEuroGirlsEscort(string urlToScrape)
        {
            try
            {
                List<string> ProfileURLs = new List<string>();

                using WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
                client.Headers.Add(HttpRequestHeader.Cookie, "over18=1");

                string htmlCode = client.DownloadString(urlToScrape);

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlCode);

                HtmlNodeCollection parentItem = doc.DocumentNode.SelectNodes("//div[contains(@class, 'list-items')]");

                if (parentItem.Count() == 1)
                {
                    HtmlNodeCollection childDivs = parentItem.First().SelectNodes(".//div");

                    foreach (HtmlNode node in childDivs)
                    {
                        HtmlNodeCollection profileURLNodes = node.SelectNodes(".//a[@href]");

                        if (profileURLNodes != null)
                        {
                            foreach (HtmlNode profileNode in profileURLNodes.Where(x => x.InnerHtml != null))
                            {
                                string hrefValue = profileNode.GetAttributeValue("href", string.Empty);

                                string url = "https://" + new Uri(urlToScrape).Host + hrefValue;
                                ProfileURLs.Add(url);
                            }
                        }
                    }

                    foreach (string url in ProfileURLs)
                    {
                        try
                        {
                            string finalInfo = "";

                            htmlCode = browser.Get(url, true, Properties.Resources.EuroGirlsEscort);

                            finalInfo += url + newLine;

                            doc = new HtmlDocument();
                            doc.LoadHtml(htmlCode);

                            HtmlNode parentProfile = doc.GetElementbyId("main-content");
                            HtmlNode descriptionNode = parentProfile.SelectSingleNode(".//div[@class='description']");
                            HtmlNode nameNode = descriptionNode.SelectSingleNode(".//h1");

                            string nameStr = nameNode.InnerText.Replace("\n", "").Replace("\r", "").Trim().Replace("  ", " ");

                            if (nameStr.Contains(","))
                            {
                                string Name = nameStr.Split(',')[0];
                                string Afiliation = nameStr.Split(',')[1];

                                finalInfo += "Name:" + Name + newLine;
                                finalInfo += "Affiliation:" + Afiliation + newLine;
                            }
                            else
                            {
                                finalInfo += "Name:" + nameStr + newLine;
                            }

                            HtmlNode profileParent = parentProfile.SelectSingleNode(".//a[contains(@class, 'js-gallery')]");

                            string imageURL = profileParent.GetAttributeValue("href", string.Empty);
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            client.DownloadFile(new Uri(imageURL), Path.Combine(AppPath, "Images/" + imageURL.Split('/').Last()));

                            HtmlNode infoNodes = parentProfile.SelectSingleNode(".//div[@class='params']");
                            HtmlNodeCollection singleInfoNodes = infoNodes.SelectNodes(".//div");

                            foreach (HtmlNode lineNode in singleInfoNodes)
                            {
                                string lineInfo = lineNode.InnerText.Replace("\n", "").Replace("\r", "").Trim().Replace("  ", " ");
                                finalInfo += lineInfo + newLine;
                            }

                            HtmlNode phoneNode = parentProfile.SelectSingleNode(".//a[contains(@class, 'js-phone')]");

                            string phoneNumber = phoneNode.InnerText.Replace("&nbsp;", " ");
                            finalInfo += "Phone number:" + phoneNumber + newLine;

                            try
                            {
                                HtmlNode ratesNodes = parentProfile.SelectSingleNode(".//div[@class='rates']");

                                if (ratesNodes != null)
                                {
                                    HtmlNode ratesTableNodes = ratesNodes.SelectSingleNode(".//tbody");

                                    HtmlNodeCollection ratesLines = ratesTableNodes.SelectNodes(".//tr");

                                    finalInfo += "Rates:";

                                    foreach (HtmlNode rateLine in ratesLines)
                                    {
                                        string rate = rateLine.InnerText.Replace("&nbsp;", " ").Replace("\n", "-").Replace("\r", "-").Trim();

                                        rate = rate.Replace("--", "-").Replace("--", "-");

                                        finalInfo += rate;

                                        if (rateLine != ratesLines.Last())
                                            finalInfo += " | ";
                                    }

                                    finalInfo += newLine;
                                }
                            }
                            catch { }

                            try
                            {
                                HtmlNode servicesNodes = parentProfile.SelectSingleNode(".//div[@class='services']");

                                if (servicesNodes != null)
                                {
                                    HtmlNode servicesTableNodes = servicesNodes.SelectSingleNode(".//tbody");

                                    HtmlNodeCollection servicesLines = servicesNodes.SelectNodes(".//tr");

                                    finalInfo += "Services:";

                                    foreach (HtmlNode serviceLine in servicesLines)
                                    {
                                        string service = serviceLine.InnerText.Replace("&nbsp;", " ").Replace("\n", "-").Replace("\r", "-").Trim();

                                        service = service.Replace("--", "-").Replace("--", "-").Replace("--", "-");

                                        if (service == "-Services-Included-Extra-")
                                            continue;

                                        finalInfo += service;

                                        if (serviceLine != servicesLines.Last())
                                            finalInfo += " | ";
                                    }
                                    finalInfo += newLine;
                                }
                            }
                            catch { }

                            finalInfo = finalInfo.Trim();

                            string profilesPath = Path.Combine(AppPath, "Profiles");

                            if (!Directory.Exists(profilesPath))
                                Directory.CreateDirectory(profilesPath);

                            string ProfilePath = Path.Combine(profilesPath, nameStr);

                            if (Directory.Exists(ProfilePath))
                            {
                                Directory.Delete(ProfilePath, true);
                            }

                            Directory.CreateDirectory(ProfilePath);

                            string profileTxtPath = Path.Combine(ProfilePath, "profile.txt");

                            StreamWriter sw = new StreamWriter(profileTxtPath);
                            sw.WriteLine(finalInfo);
                            sw.Close();

                            HtmlNode imgNode = doc.GetElementbyId("js-gallery");

                            HtmlNodeCollection imgsNodes = imgNode.SelectNodes(".//a[@class='js-gallery']");

                            int i = 0;

                            foreach (HtmlNode imgElm in imgsNodes)
                            {
                                string imgURL = imgElm.GetAttributeValue("href", string.Empty);

                                client.DownloadFile(imgURL, Path.Combine(ProfilePath, $"{i}.jpg"));
                                i++;
                            }
                        }
                        catch (ThreadAbortException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            LogError("Url invalid: " + url + ". Error: " + ex.ToString());
                            break;
                        }
                    }
                }
                else
                {
                    LogError($"Invalid parent item count, expected 1 got {parentItem.Count()}");
                }
            }
            catch (ThreadAbortException)
            {
                //nothing, stopped
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return true;
        }

        private bool ScrapeTopEscortBabes(string urlToScrape)
        {
            try
            {
                List<string> ProfileURLs = new List<string>();

                using WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0");
                client.Headers.Add(HttpRequestHeader.Cookie, "plus18=1");

                string htmlCode = client.DownloadString(urlToScrape);

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlCode);

                HtmlNode parentNode = doc.GetElementbyId("homepage_right");
                HtmlNodeCollection itemParentsNodes = parentNode.SelectNodes(".//div[contains(@class, 'items')]");

                if (itemParentsNodes.Count() == 2)
                {
                    foreach (HtmlNode mainNode in itemParentsNodes)
                    {
                        HtmlNodeCollection profilesNodes = mainNode.SelectNodes(".//li");

                        if (profilesNodes != null)
                        {
                            foreach (HtmlNode singleProfileNode in profilesNodes)
                            {
                                HtmlNode hrefNode = singleProfileNode.SelectSingleNode(".//a[@href]");
                                string hrefValue = hrefNode.GetAttributeValue("href", string.Empty);

                                if (string.IsNullOrWhiteSpace(hrefValue))
                                    continue;

                                ProfileURLs.Add(hrefValue);
                            }
                        }
                    }
                    
                    foreach (string url in ProfileURLs)
                    {
                        try
                        {
                            string finalInfo = "";

                            htmlCode = browser.Get(url, true, Properties.Resources.TopEscortBabes);

                            Clipboard.SetText(htmlCode);
                            CMBox.Show("1");

                            finalInfo += url + newLine;

                            doc = new HtmlDocument();
                            doc.LoadHtml(htmlCode);

                            HtmlNode mainParentNode = doc.GetElementbyId("homepage");

                            HtmlNode headerNode = mainParentNode.SelectSingleNode(".//div[@class='profile-cover']");
                            HtmlNode titleNode = headerNode.SelectSingleNode(".//h2[@class='header-title']");

                            string profileName = titleNode.InnerText.Replace("\n", "").Replace("\r", "").Trim().Replace("  ", " ");

                            finalInfo += "Name:" + profileName + newLine;

                            try
                            {
                                HtmlNode afiliationNode = doc.GetElementbyId("accord-agency");

                                if (afiliationNode == null)
                                {
                                    finalInfo += "Affiliation:Independent" + newLine;
                                }
                                else
                                {
                                    HtmlNode afiliationBodyNode = afiliationNode.SelectSingleNode(".//div");
                                    HtmlNode afiliationNameNode = afiliationBodyNode.SelectSingleNode(".//h4");

                                    if (afiliationNameNode == null)
                                    {
                                        finalInfo += "Affiliation:Independent" + newLine;
                                    }
                                    else
                                    {
                                        string afiliation = afiliationNameNode.InnerText.Replace("\n", "").Replace("\r", "").Trim().Replace("  ", " ");
                                        finalInfo += "Affiliation:" + afiliation + newLine;
                                    }
                                }
                            }
                            catch { }

                            HtmlNode personalNode = doc.GetElementbyId("accord-personal-data");
                            HtmlNode detailsNode = personalNode.SelectSingleNode(".//div[contains(@class, 'detail-block-body')]");

                            HtmlNodeCollection detailLineNode = detailsNode.SelectNodes(".//div[@class='personal-data-item']");

                            foreach (HtmlNode lineNode in detailLineNode)
                            {
                                string infoLine = lineNode.InnerText.Replace("\n", "").Replace("\r", "").Trim().Replace("  ", " ");

                                RegexOptions options = RegexOptions.None;
                                Regex regex = new Regex("[ ]{2,}", options);
                                infoLine = regex.Replace(infoLine, ":");

                                infoLine = infoLine.Replace(":•:", ",");

                                finalInfo += infoLine + newLine;
                            }

                            try
                            {
                                HtmlNode priceNode = doc.GetElementbyId("prices");
                                HtmlNodeCollection priceListNode = priceNode.SelectNodes(".//div[@class='price-item']");

                                if (priceListNode != null)
                                {
                                    finalInfo += "Prices:";

                                    foreach (HtmlNode lineNode in priceListNode)
                                    {
                                        string infoLine = lineNode.InnerText.Replace("\n", "").Replace("\r", "").Trim().Replace("  ", " ");

                                        RegexOptions options = RegexOptions.None;
                                        Regex regex = new Regex("[ ]{2,}", options);
                                        infoLine = regex.Replace(infoLine, ":");

                                        infoLine = "-" + infoLine.Replace(" Price:", "-").Replace(":", " ") + "-";

                                        finalInfo += infoLine;

                                        if (lineNode != priceListNode.Last())
                                        {
                                            finalInfo += " | ";
                                        }
                                    }

                                    finalInfo += newLine;
                                }
                            }
                            catch { }

                            finalInfo = finalInfo.Trim();

                            string profilesPath = Path.Combine(AppPath, "Profiles");

                            if (!Directory.Exists(profilesPath))
                                Directory.CreateDirectory(profilesPath);

                            string ProfilePath = Path.Combine(profilesPath, profileName);

                            if (Directory.Exists(ProfilePath))
                                Directory.Delete(ProfilePath, true);

                            Directory.CreateDirectory(ProfilePath);

                            string profileTxtPath = Path.Combine(ProfilePath, "profile.txt");

                            StreamWriter sw = new StreamWriter(profileTxtPath);
                            sw.WriteLine(finalInfo);
                            sw.Close();

                            try
                            {
                                HtmlNode mainImageNode = doc.GetElementbyId("profile-details-right");
                                

                                Clipboard.SetText(mainImageNode.OuterHtml);
                                CMBox.Show("1");

                                HtmlNode imagesParentNode = mainImageNode.SelectSingleNode(".//div[@class='photos-wrapper']");

                                HtmlNodeCollection imagesNode = imagesParentNode.SelectNodes(".//a[@class='ilightbox']");

                                foreach (HtmlNode singleImageNode in imagesNode)
                                {
                                    string hrefValue = singleImageNode.GetAttributeValue("href", string.Empty);
                                    Log(hrefValue);
                                }
                            }
                            catch { }
                            
                        }
                        catch (ThreadAbortException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            LogError("Url invalid: " + url + ". Error: " + ex.ToString());
                            break;
                        }
                    }
                }
                else
                {
                    LogError($"Invalid parent item count, expected 2 got {itemParentsNodes.Count()}");
                }
            }
            catch (ThreadAbortException)
            {
                //nothing, stopped
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return true;
        }
        #endregion

        #region "Handles"
        private void FrmMain_Load(object sender, EventArgs e)
        {
            SetCueText(TxtAddURL, "https://...");
            LoadURLs();
        }

        private void BtnAddURL_Click(object sender, EventArgs e)
        {
            string urlToAdd = TxtAddURL.Text;
            bool result = Uri.TryCreate(urlToAdd, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
            {
                Sites.URLs.Add(new WebSite(urlToAdd, true, "waiting"));
                RefreshUrlList();
                SaveURLs();
                TxtAddURL.Text = string.Empty;
            }
            else
                CMBox.Show("Warning", "Invalid URL, check and try again", Style.Warning, Buttons.OK);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (!Scraping)
            {
                BtnStart.Text = "Stop";
                LabelStatus.Text = "Getting profiles...";
                StartScape();
            }
            else
            {
                BtnStart.Text = "Start";
                LabelStatus.Text = "Idle...";
                StopScrape(true);
            }
        }

        private void ContextListURLsCopy_Click(object sender, EventArgs e)
        {
            if (ListURLs.SelectedItems.Count == 1)
            {
                string url = ListURLs.SelectedItems[0].SubItems[0].Text;

                if (!string.IsNullOrWhiteSpace(url))
                    Clipboard.SetText(url);
            }
            else
                CMBox.Show("Warning", "One Website must be selected", Style.Warning, Buttons.OK);
        }

        private void ContextListURLsOpen_Click(object sender, EventArgs e)
        {
            if (ListURLs.SelectedItems.Count == 1)
            {
                string url = ListURLs.SelectedItems[0].SubItems[0].Text;

                if (!string.IsNullOrWhiteSpace(url))
                    Process.Start(url);
            }
            else
                CMBox.Show("Warning", "One Website must be selected", Style.Warning, Buttons.OK);
        }
        #endregion
    }
}