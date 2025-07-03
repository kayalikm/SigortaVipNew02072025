using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CefSharp.WinForms;

namespace SigortaVipNew.Helpers
{
    public class ResourceManager : IDisposable
    {
        private readonly List<IDisposable> _disposables;
        private readonly List<ChromiumWebBrowser> _browsers;
        private bool _disposed = false;

        public ResourceManager()
        {
            _disposables = new List<IDisposable>();
            _browsers = new List<ChromiumWebBrowser>();
        }

        // Browser'ı kaydet ve takip et
        public void RegisterBrowser(ChromiumWebBrowser browser)
        {
            if (browser != null && !_browsers.Contains(browser))
            {
                _browsers.Add(browser);
                ErrorLogger.LogError($"Browser registered. Total browsers: {_browsers.Count}");
            }
        }

        // IDisposable nesneyi kaydet ve takip et
        public void RegisterDisposable(IDisposable disposable)
        {
            if (disposable != null && !_disposables.Contains(disposable))
            {
                _disposables.Add(disposable);
            }
        }

        // Browser'ı güvenli şekilde kapat
        public void DisposeBrowser(ChromiumWebBrowser browser)
        {
            if (browser != null && _browsers.Contains(browser))
            {
                try
                {
                    // Parent control'dan kaldır
                    browser.Parent?.Controls.Remove(browser);

                    // Browser'ı dispose et
                    browser.Dispose();

                    // Listeden çıkar
                    _browsers.Remove(browser);

                    ErrorLogger.LogError($"Browser disposed. Remaining browsers: {_browsers.Count}");
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex, "Browser dispose hatası");
                }
            }
        }

        // Tüm kaynakları temizle
        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    // Tüm browser'ları temizle
                    foreach (var browser in _browsers.ToArray())
                    {
                        DisposeBrowser(browser);
                    }
                    _browsers.Clear();

                    // Tüm disposable nesneleri temizle
                    foreach (var disposable in _disposables.ToArray())
                    {
                        try
                        {
                            disposable?.Dispose();
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.LogError(ex, "Resource dispose hatası");
                        }
                    }
                    _disposables.Clear();

                    ErrorLogger.LogError("ResourceManager disposed successfully");
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex, "ResourceManager dispose hatası");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }

        ~ResourceManager()
        {
            Dispose();
        }
    }
}