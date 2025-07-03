using CefSharp;
using CefSharp.Handler;

namespace SigortaVipNew.Handler
{
    public class CustomRequestHandler : RequestHandler
    {
        private readonly string _username;
        private readonly string _password;

        public CustomRequestHandler(string username, string password)
        {
            _username = username;
            _password = password;
        }

        protected override bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser,
            string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            if (isProxy)
            {
                // Sadece geçerli kullanıcı adı ve şifre varsa kimlik doğrulaması yap
                if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_password))
                {
                    callback.Continue(_username, _password);
                    return true;
                }
                else
                {
                    // Kimlik bilgisi yoksa işlemi iptal et (bazı durumlarda gerekebilir)
                    callback.Cancel();
                    return false;
                }
            }
            return base.GetAuthCredentials(chromiumWebBrowser, browser, originUrl, isProxy, host, port, realm, scheme, callback);
        }

        // Yeni sekme engelleme özelliği - sadece bu metodu ekleyin
        protected override bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            // Yeni sekme, pencere veya popup açılmasını engelle
            // Bunun yerine mevcut sekmede aç
            if (targetDisposition == WindowOpenDisposition.NewBackgroundTab ||
                targetDisposition == WindowOpenDisposition.NewForegroundTab ||
                targetDisposition == WindowOpenDisposition.NewWindow ||
                targetDisposition == WindowOpenDisposition.NewPopup)
            {
                // Mevcut tarayıcıda URL'yi yükle
                chromiumWebBrowser.Load(targetUrl);
                return true; // Varsayılan davranışı engelle
            }

            return base.OnOpenUrlFromTab(chromiumWebBrowser, browser, frame, targetUrl, targetDisposition, userGesture);
        }
    }
}