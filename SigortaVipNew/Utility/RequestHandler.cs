using CefSharp;
using System.Security.Cryptography.X509Certificates;

public class AuthenticatedRequestHandler : IRequestHandler
{
    private readonly string _username;
    private readonly string _password;

    public AuthenticatedRequestHandler(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser,
                                   string originUrl, bool isProxy, string host,
                                   int port, string realm, string scheme, IAuthCallback callback)
    {
        if (isProxy)
        {
            callback.Continue(_username, _password);
            return true;
        }
        return false;
    }

    public IResourceRequestHandler GetResourceRequestHandler(
        IWebBrowser chromiumWebBrowser,
        IBrowser browser,
        IFrame frame,
        IRequest request,
        bool isNavigation,
        bool isDownload,
        string requestInitiator,
        ref bool disableDefaultHandling)
    {
        return null;
    }

    public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser,
                               IFrame frame, IRequest request,
                               bool userGesture, bool isRedirect) => false;

    public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser,
                                   CefErrorCode errorCode, string requestUrl,
                                   ISslInfo sslInfo, IRequestCallback callback) => false;

    public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser) { }

    public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser,
                                 IFrame frame, string targetUrl,
                                 WindowOpenDisposition targetDisposition,
                                 bool userGesture) => false;

    public void OnPluginCrashed(IWebBrowser chromiumWebBrowser,
                                IBrowser browser, string pluginPath)
    { }

    public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser,
                               string originUrl, long newSize,
                               IRequestCallback callback) => false;

    public void OnRenderViewReady(IWebBrowser chromiumWebBrowser,
                                  IBrowser browser)
    { }

    public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser,
                                          IBrowser browser, bool isProxy,
                                          string host, int port,
                                          X509Certificate2Collection certificates,
                                          ISelectClientCertificateCallback callback) => false;

    public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser,
                                          IBrowser browser,
                                          CefTerminationStatus status,
                                          int exitCode,
                                          string message)
    { }
}
