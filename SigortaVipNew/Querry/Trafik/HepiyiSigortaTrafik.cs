using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Constant;
using SigortaVip.FiyatSorgulamaFactory.Interface;
using SigortaVip.Models;
using SigortaVip.Models.Teminatlar;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVip.FiyatSorgulamaFactory.Concrete
{
    public class HepiyiSigortaTrafikFiyat : IFiyatSorgu
    {
        private const int DEFAULT_DELAY = 500;
        private const int EXTENDED_DELAY = 3000;
        private const int TIMEOUT_SECONDS = 120;

        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            try
            {
                // İptal kontrolü ve başlangıç progress
                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(5);

                // Giriş kontrolü
                if (!await IsUserLoggedIn(browser))
                {
                    return CreateErrorResult("Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır");
                }

                progress?.Report(15);
                cancellationToken.ThrowIfCancellationRequested();

                // Ana sayfaya yönlendir
                await browser.LoadUrlAsync("https://acente.hepiyi.com.tr/Proposal/Auto?pNo=301");

                progress?.Report(25);
                cancellationToken.ThrowIfCancellationRequested();

                // JavaScript event'lerini hazırla
                await InitializeJavaScriptEvents(browser);

                progress?.Report(35);
                cancellationToken.ThrowIfCancellationRequested();

                // Form doldurma işlemleri
                await FillUserInformation(browser, info);

                progress?.Report(70);
                cancellationToken.ThrowIfCancellationRequested();

                // Sorgula butonuna tıkla
                await browser.EvaluateScriptAsync("document.getElementById(\"btnCreateProposalNewVehicle\").click();");

                progress?.Report(80);
                cancellationToken.ThrowIfCancellationRequested();

                // Loader bekleme ve sonuç alma
                await WaitForLoader(browser);

                progress?.Report(90);
                cancellationToken.ThrowIfCancellationRequested();

                // Hata kontrolü
                var errorMessage = await GetErrorMessage(browser);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return CreateErrorResult(errorMessage);
                }

                // Fiyat bilgisini al
                var fiyatBilgisi = await GetPriceInformation(browser);
                
                if (fiyatBilgisi != null)
                {
                    progress?.Report(100);
                }
                
                return fiyatBilgisi ?? CreateErrorResult("Fiyat bilgisi alınamadı");
            }
            catch (Exception ex)
            {
                return CreateErrorResult($"Hata: {ex.Message}");
            }
        }

        private async Task<bool> IsUserLoggedIn(ChromiumWebBrowser browser)
        {
            var loginCheck = await browser.EvaluateScriptAsync(@"
                (function() {
                    var loginBtn = document.querySelector(""[value='login']"");
                    return loginBtn ? loginBtn.innerText : '';
                })();
            ");

            return string.IsNullOrEmpty(loginCheck.Result?.ToString());
        }

        private async Task InitializeJavaScriptEvents(ChromiumWebBrowser browser)
        {
            await browser.EvaluateScriptAsync(@"
                var changeEvent = new Event('change'),
                    inputEvent = new Event('input'),
                    focusEvent = new Event('focus'),
                    blurEvent = new Event('blur');
            ");
        }

        private async Task FillUserInformation(ChromiumWebBrowser browser, KullaniciBilgileri info)
        {
            // TC Kimlik No
            await SetInputValue(browser, "Insured_CitizenshipNo", info.txtKimlikNo);
            await WaitForLoader(browser);

            // Plaka
            await SetInputValue(browser, "txtAutoInputParamPlate", info.txtPlakaNo);
            await WaitForLoader(browser);

            // Hata kontrolü
            var errorMessage = await GetErrorMessage(browser);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }

            // Tescil
            await SetInputValue(browser, "txtAutoInputParamRegisterNo", info.txtSeriNo);
            await Task.Delay(EXTENDED_DELAY);

            // Araç Kodu
            await SetInputValue(browser, "txtAutoInputQ10272", info.txtAracKodu);
            await Task.Delay(DEFAULT_DELAY * 2);

            // Kullanım Tarzı
            await SetUsageType(browser, info.txtkullanımtarzı);
            await Task.Delay(DEFAULT_DELAY * 2);
        }

        private async Task SetInputValue(ChromiumWebBrowser browser, string elementId, string value)
        {
            var script = $@"
                var element = document.getElementById('{elementId}');
                if (element) {{
                    element.value = '{value}';
                    element.dispatchEvent(changeEvent);
                    element.dispatchEvent(inputEvent);
                    element.dispatchEvent(focusEvent);
                    element.dispatchEvent(blurEvent);
                }}
            ";
            await browser.EvaluateScriptAsync(script);
        }

        private async Task SetUsageType(ChromiumWebBrowser browser, string usageType)
        {
            string value;
            switch (usageType.ToUpperInvariant())
            {
                case "OTOMOBİL":
                    value = "1";
                    break;
                case "KAMYONET":
                    value = "6";
                    break;
                default:
                    value = "1"; // Default değer
                    break;
            }

            await SetSelectValue(browser, "ddAutoInputQ10004", value);
        }

        private async Task SetSelectValue(ChromiumWebBrowser browser, string elementId, string value)
        {
            var script = $@"
                var element = document.getElementById('{elementId}');
                if (element) {{
                    element.value = '{value}';
                    element.dispatchEvent(changeEvent);
                    element.dispatchEvent(inputEvent);
                    element.dispatchEvent(focusEvent);
                    element.dispatchEvent(blurEvent);
                }}
            ";
            await browser.EvaluateScriptAsync(script);
        }

        private async Task WaitForLoader(ChromiumWebBrowser browser, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                await Task.Delay(DEFAULT_DELAY);
                cancellationToken.ThrowIfCancellationRequested();
                var loader = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var overlay = document.querySelector('.blockui-overlay');
                        return overlay ? overlay.style[0] : null;
                    })();
                ");

                if (loader.Result == null)
                    break;
            }
        }

        private async Task<string> GetErrorMessage(ChromiumWebBrowser browser)
        {
            var errorResult = await browser.EvaluateScriptAsync(@"
                (function() {
                    var errorElement = document.getElementById('swal2-html-container');
                    return errorElement ? errorElement.innerText : null;
                })();
            ");

            return errorResult.Result?.ToString();
        }

        private async Task<FiyatBilgisi> GetPriceInformation(ChromiumWebBrowser browser)
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed.TotalSeconds < TIMEOUT_SECONDS)
            {
                var priceResult = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var priceElement = document.getElementById('hi_premium_grossPremium_info');
                        return priceElement ? priceElement.innerText : null;
                    })();
                ");

                var priceText = priceResult.Result?.ToString();

                if (!string.IsNullOrEmpty(priceText))
                {
                    stopwatch.Stop();
                    return new FiyatBilgisi
                    {
                        BrutPrim = priceText,
                        Durum = "Tamamlandı",
                        FirmaAdi = InsuranceConstants.HepIyiSigorta,
                        Komisyon = "", // Komisyon bilgisi de alınabilir
                        TeklifNo = ""
                    };
                }

                await Task.Delay(DEFAULT_DELAY);
            }

            stopwatch.Stop();
            return CreateErrorResult("2 Dakika boyunca teklif alınamadı");
        }

        private FiyatBilgisi CreateErrorResult(string errorMessage)
        {
            return new FiyatBilgisi
            {
                BrutPrim = "",
                Durum = errorMessage,
                FirmaAdi = InsuranceConstants.HepIyiSigorta,
                Komisyon = "",
                TeklifNo = ""
            };
        }
    }
}