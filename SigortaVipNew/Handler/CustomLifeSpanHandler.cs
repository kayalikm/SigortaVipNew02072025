using CefSharp;
using CefSharp.Handler;

namespace SigortaVipNew.Handler
{
    public class CustomLifeSpanHandler : LifeSpanHandler
    {
        protected override bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture,
            IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings,
            ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;

            // Tüm popup ve yeni pencere açılmalarını engelle
            if (!string.IsNullOrEmpty(targetUrl))
            {
                // Mevcut tarayıcıda URL'yi yükle
                chromiumWebBrowser.Load(targetUrl);
                System.Diagnostics.Debug.WriteLine($"Popup engellendi, mevcut sekmede açıldı: {targetUrl}");
            }

            // true döndürerek popup'ın açılmasını engelle
            return true;
        }
    }
}