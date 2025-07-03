using CefSharp;
using CefSharp.WinForms;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace SigortaVip.Business
{
    class CookieManager
    {
        private static CookieManager cookieManager = null;
        public CookieManager()
        {

        }

        public static CookieManager getInstance()
        {
            if (cookieManager == null)
                return new CookieManager();

            return cookieManager;
        }

        public void setCookie(string url, CefSharp.Cookie cookie, ChromiumWebBrowser browser)
        {
            BrowserManager.getInstance();
            var cookieManager = browser.GetCookieManager();
            cookieManager.SetCookie(url, cookie);
        }

        public void setExplorerCookie(string url, CefSharp.Cookie cookie, WebBrowser explorer)
        {
            InternetSetCookie(url + cookie.Path, cookie.Name, cookie.Value);
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

    }
}
