using CefSharp;
using CefSharp.WinForms;
using System.Collections.Generic;

namespace SigortaVip.Business
{
    class Browser
    {
        public static List<WebPage> webPageList = new List<WebPage>();

        public static CefSettings settings = null;
        public Browser()
        {
            if(settings == null)
            {
                settings = new CefSettings();
                settings.PersistSessionCookies = true;
                settings.CefCommandLineArgs["disable-features"] += ",SameSiteByDefaultCookies";
                Cef.Initialize(settings);
            }

        }
    }
}
