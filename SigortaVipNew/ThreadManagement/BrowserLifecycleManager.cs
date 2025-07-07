// ===== .NET Framework 4.8 Uyumlu BrowserLifecycleManager.cs =====
// Bu dosyayı projenize ekleyin: SigortaVipNew/ThreadManagement/BrowserLifecycleManager.cs

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Utility;
using SigortaVipNew.Helpers;
using System.Security.Cryptography.X509Certificates;

namespace SigortaVipNew.ThreadManagement
{
    public class BrowserLifecycleManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ManagedBrowser> _browsers = new ConcurrentDictionary<string, ManagedBrowser>();
        private readonly SemaphoreSlim _creationSemaphore = new SemaphoreSlim(3, 3); // Max 3 eşzamanlı browser
        private readonly System.Threading.Timer _inactivityTimer; // ✅ Explicit namespace
        private readonly object _disposeLock = new object();
        private bool _disposed = false;

        public BrowserLifecycleManager()
        {
            // Her 2 dakikada inactive browser'ları temizle
            _inactivityTimer = new System.Threading.Timer(CleanupInactiveBrowsers, null,
                TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));

            ErrorLogger.LogError("BrowserLifecycleManager initialized");
        }

        public async Task<ChromiumWebBrowser> GetOrCreateBrowserAsync(
            string tabKey,
            string url,
            string proxyServer = null,
            string username = null,
            string password = null)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(BrowserLifecycleManager));

            // Mevcut browser'ı kontrol et
            ManagedBrowser existing;
            if (_browsers.TryGetValue(tabKey, out existing) && !existing.Browser.IsDisposed)
            {
                existing.UpdateLastAccessed();
                ErrorLogger.LogError(string.Format("Existing browser reused: {0}", tabKey));
                return existing.Browser;
            }

            // Yeni browser oluştur (sınırlı eşzamanlılık ile)
            await _creationSemaphore.WaitAsync();
            try
            {
                return await CreateBrowserInternalAsync(tabKey, url, proxyServer, username, password);
            }
            finally
            {
                _creationSemaphore.Release();
            }
        }

        private async Task<ChromiumWebBrowser> CreateBrowserInternalAsync(
            string tabKey, string url, string proxyServer, string username, string password)
        {
            try
            {
                ErrorLogger.LogError(string.Format("Creating new browser: {0}", tabKey));

                // Request context oluştur (proxy ile)
                IRequestContext requestContext = null; // ✅ IRequestContext kullan
                if (!string.IsNullOrEmpty(proxyServer))
                {
                    var requestContextBuilder = RequestContext.Configure()
                        .WithProxyServer(proxyServer);
                    requestContext = requestContextBuilder.Create();
                }

                // Browser oluştur
                var browser = new ChromiumWebBrowser
                {
                    Dock = DockStyle.Fill,
                    BrowserSettings = new BrowserSettings
                    {
                        DefaultEncoding = "UTF-8",
                        Javascript = CefState.Enabled,
                        LocalStorage = CefState.Enabled,
                        ImageLoading = CefState.Enabled,
                        WebGl = CefState.Disabled, // WebGL devre dışı - performans için
                        // ✅ Plugins property'si CefSharp'ın eski versiyonlarında yok - kaldır
                    },
                    RequestContext = requestContext
                };

                // Request handler ekle (authentication için)
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    browser.RequestHandler = new OptimizedRequestHandler(username, password);
                }

                // Disposed olayını dinle
                browser.Disposed += (s, e) =>
                {
                    ManagedBrowser removed;
                    _browsers.TryRemove(tabKey, out removed);
                    ErrorLogger.LogError(string.Format("Browser disposed: {0}", tabKey));
                };

                // Managed browser oluştur ve kaydet
                var managedBrowser = new ManagedBrowser(browser, tabKey, url);
                _browsers.TryAdd(tabKey, managedBrowser);

                ErrorLogger.LogError(string.Format("Browser created successfully: {0}", tabKey));

                return browser;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, string.Format("Browser creation error for {0}", tabKey));
                throw;
            }
        }

        public async Task<bool> SafeDisposeBrowserAsync(string tabKey)
        {
            ManagedBrowser managedBrowser;
            if (!_browsers.TryRemove(tabKey, out managedBrowser))
                return false;

            try
            {
                await managedBrowser.SafeDisposeAsync();
                ErrorLogger.LogError(string.Format("Browser safely disposed: {0}", tabKey));
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, string.Format("Safe dispose error for {0}", tabKey));
                return false;
            }
        }

        private void CleanupInactiveBrowsers(object state)
        {
            if (_disposed) return;

            try
            {
                var cutoffTime = DateTime.Now.AddMinutes(-10); // 10 dakika inactive
                var inactiveBrowsers = _browsers
                    .Where(kvp => kvp.Value.LastAccessed < cutoffTime)
                    .ToList();

                foreach (var kvp in inactiveBrowsers)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await SafeDisposeBrowserAsync(kvp.Key);
                            ErrorLogger.LogError(string.Format("Inactive browser disposed: {0}", kvp.Key));
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.LogError(ex, string.Format("Cleanup error for {0}", kvp.Key));
                        }
                    });
                }

                if (inactiveBrowsers.Count > 0)
                {
                    ErrorLogger.LogError(string.Format("Cleanup completed - Active: {0}, Disposed: {1}",
                        _browsers.Count, inactiveBrowsers.Count));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Cleanup timer error");
            }
        }

        public int GetActiveBrowserCount()
        {
            return _browsers.Count;
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_disposed) return;
                _disposed = true;
            }

            try
            {
                if (_inactivityTimer != null)
                {
                    _inactivityTimer.Dispose();
                }

                if (_creationSemaphore != null)
                {
                    _creationSemaphore.Dispose();
                }

                // Tüm browser'ları dispose et
                var disposeTasks = _browsers.Values.Select(mb => mb.SafeDisposeAsync()).ToArray();
                Task.WaitAll(disposeTasks, 10000); // 10 saniye timeout

                _browsers.Clear();
                ErrorLogger.LogError("BrowserLifecycleManager disposed");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "BrowserLifecycleManager dispose error");
            }
        }
    }

    // ManagedBrowser wrapper sınıfı
    public class ManagedBrowser
    {
        public ChromiumWebBrowser Browser { get; private set; }
        public string TabKey { get; private set; }
        public string InitialUrl { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastAccessed { get; private set; }

        private readonly object _disposeLock = new object();
        private bool _disposed = false;

        public ManagedBrowser(ChromiumWebBrowser browser, string tabKey, string url)
        {
            Browser = browser;
            TabKey = tabKey;
            InitialUrl = url;
            Created = DateTime.Now;
            LastAccessed = DateTime.Now;
        }

        public void UpdateLastAccessed()
        {
            LastAccessed = DateTime.Now;
        }

        public async Task SafeDisposeAsync()
        {
            lock (_disposeLock)
            {
                if (_disposed) return;
                _disposed = true;
            }

            try
            {
                if (Browser != null && !Browser.IsDisposed)
                {
                    // Browser'ı güvenli şekilde kapat
                    if (Browser.IsBrowserInitialized)
                    {
                        var browserHost = Browser.GetBrowser();
                        if (browserHost != null)
                        {
                            browserHost.CloseBrowser(true);
                        }

                        // Browser'ın kapanmasını bekle
                        for (int i = 0; i < 50; i++) // Max 5 saniye bekle
                        {
                            if (Browser.IsDisposed) break;
                            await Task.Delay(100);
                        }
                    }

                    // Request context'i temizle
                    if (Browser.RequestContext != null)
                    {
                        Browser.RequestContext.Dispose();
                    }

                    // Browser'ı dispose et
                    Browser.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, string.Format("ManagedBrowser dispose error: {0}", TabKey));
            }
            finally
            {
                Browser = null;
            }
        }
    }

    // ✅ .NET Framework 4.8 uyumlu Request Handler
    // ===== DÜZELTİLMİŞ OptimizedRequestHandler =====
    // BrowserLifecycleManager.cs dosyasındaki OptimizedRequestHandler sınıfını bu kodla değiştirin

    public class OptimizedRequestHandler : IRequestHandler
    {
        private readonly string _username;
        private readonly string _password;

        public OptimizedRequestHandler(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser,
            string originUrl, bool isProxy, string host, int port, string realm,
            string scheme, IAuthCallback callback)
        {
            callback.Continue(_username, _password);
            return true;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser,
            IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition,
            bool userGesture)
        {
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser,
            CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            callback.Continue(true); // SSL hatalarını yoksay
            return true;
        }

        // ✅ CefSharp versiyonunuza göre doğru method signature
        // Eğer hala hata alıyorsanız bu metodu tamamen silin
        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser,
            CefTerminationStatus status, int errorCode, string errorString)
        {
            // Render process terminated handling
            ErrorLogger.LogError(string.Format("Render process terminated: Status={0}, Code={1}, Error={2}",
                status, errorCode, errorString));
        }

        // Diğer minimal implementasyonlar - bunları olduğu gibi bırakın
        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath) { }
        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser) { }
        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser) { }
        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback) { return false; }
        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl) { }
        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request) { return false; }
        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength) { }
        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) { return null; }
        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) { return false; }
    }
}