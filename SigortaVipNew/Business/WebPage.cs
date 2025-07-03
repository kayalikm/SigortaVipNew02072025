using CefSharp.WinForms;
using System.Collections.Generic;

namespace SigortaVip.Business
{
    class WebPage
    {
        public ChromiumWebBrowser browser { get; set; }
        public string insuranceCompany { get; set; }
        public string insuranceCompanyPageUrl { get; set; }

        public List<CefSharp.Cookie> initialCookies = new List<CefSharp.Cookie>();
        public long insuranceCompanyId { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public WebPage(string url, string insuranceCompany, List<CefSharp.Cookie> initialCookies, long insuranceCompanyId, string username, string password,ChromiumWebBrowser browser = null)
        {
            //this.browser = browser == null ? BrowserManager.Instance.createWebBrowser(url) : browser;
            //this.browser = BrowserManager.Instance.createWebBrowser(url);
            this.browser = BrowserManager.getInstance().createWebBrowser(url);


            this.insuranceCompany = insuranceCompany;

            if(initialCookies != null)
            {
                foreach (var initialCookie in initialCookies)
                {
                    this.initialCookies.Add(initialCookie);
                }
            }
            this.insuranceCompanyId = insuranceCompanyId;
            this.username = username;
            this.password = password;
            this.insuranceCompanyPageUrl = url;
        }

        public WebPage()
        {
        }

        public void setinitialCookies(List<CefSharp.Cookie> newCookies)
        {
            this.initialCookies.Clear();
            foreach (var newCookie in newCookies)
            {
                this.initialCookies.Add(newCookie);
            }
        }
    }
}
