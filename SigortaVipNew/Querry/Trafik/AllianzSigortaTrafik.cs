using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Business;
using SigortaVip.Constant;
using SigortaVip.FiyatSorgulamaFactory.Interface;
using SigortaVip.Models;
using SigortaVip.Models.Teminatlar;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVip.FiyatSorgulamaFactory.Concrete
{
    public class AllianzSigortaTrafikFiyat : IFiyatSorgu
    {
        private const int DefaultDelay = 1000;
        private const int MaxWaitTimeSeconds = 120;
        private const string AllianzUrl = "https://pci.allianz.com.tr/common-ui/#/home/index?cHJvZHVjdD10cmFmZmlj=";

        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            try
            {
                // İptal kontrolü
                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(5);

                // İlk kontrol - login durumu
                if (await IsLoginRequired(browser))
                {
                    return CreateErrorResult("Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır");
                }

                progress?.Report(15);
                cancellationToken.ThrowIfCancellationRequested();

                // Sayfayı yükle
                await browser.LoadUrlAsync(AllianzUrl);

                // Giriş kontrolü
                if (await IsLoginRequired(browser))
                {
                    return CreateErrorResult("Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır");
                }

                progress?.Report(25);
                cancellationToken.ThrowIfCancellationRequested();

                // Sayfa yüklenmesini bekle
                await WaitForPageLoad(browser);

                progress?.Report(35);
                cancellationToken.ThrowIfCancellationRequested();

                // Form doldurma işlemleri
                await FillUserInformation(browser, info);
                progress?.Report(45);
                cancellationToken.ThrowIfCancellationRequested();

                await FillBirthDate(browser, info.txtDogumTar);
                progress?.Report(55);
                cancellationToken.ThrowIfCancellationRequested();

                await FillPlateInformation(browser, info);
                progress?.Report(65);
                cancellationToken.ThrowIfCancellationRequested();

                // Devam butonuna tıkla
                await browser.EvaluateScriptAsync("document.getElementById('searchContinueButton').click();");
                await Task.Delay(2000);

                progress?.Report(75);
                cancellationToken.ThrowIfCancellationRequested();

                // Araç bilgilerinin yüklenmesini bekle
                //await WaitForVehicleInfo(browser);

                progress?.Report(85);
                cancellationToken.ThrowIfCancellationRequested();

                // İlçe seçimi
                await SelectDistrict(browser);

                progress?.Report(90);
                cancellationToken.ThrowIfCancellationRequested();

                // Paket sayfasına geç
                await browser.EvaluateScriptAsync("document.getElementById('packageContinueButton').click();");

                progress?.Report(95);
                cancellationToken.ThrowIfCancellationRequested();

                // Fiyat sonucunu bekle ve al
                var result = await WaitForPriceResult(browser);
                progress?.Report(100);
                
                return result;
            }
            catch (Exception ex)
            {
                return CreateErrorResult($"Beklenmeyen bir hata oluştu: {ex.Message}");
            }
        }

        private async Task<bool> IsLoginRequired(ChromiumWebBrowser browser)
        {
            var loginCheck1 = await browser.EvaluateScriptAsync("document.querySelector(\"[value='login']\")?.innerText");
            var loginCheck2 = await browser.EvaluateScriptAsync("document.querySelector(\"[value='Giriş']\")?.value");

            return (loginCheck1.Result != null && !string.IsNullOrEmpty(loginCheck1.Result.ToString())) ||
                   (loginCheck2.Result != null && !string.IsNullOrEmpty(loginCheck2.Result.ToString()));
        }

        private async Task WaitForPageLoad(ChromiumWebBrowser browser)
        {
            await Task.Delay(DefaultDelay);

            var spinner = await browser.EvaluateScriptAsync("document.getElementById('iconSpinnerChild')");
            while (spinner.Result != null)
            {
                await Task.Delay(200);
                spinner = await browser.EvaluateScriptAsync("document.getElementById('iconSpinnerChild')");
            }
        }

        private async Task FillUserInformation(ChromiumWebBrowser browser, KullaniciBilgileri info)
        {
            // Event oluşturma fonksiyonunu tanımla
            await browser.EvaluateScriptAsync(@"
                var createEvent = function(name) { 
                    var event = document.createEvent('Event'); 
                    event.initEvent(name, true, true); 
                    return event; 
                }");

            // Kimlik numarasını doldur
            await FillInputField(browser, "nx-input-1", info.txtKimlikNo);
        }

        private async Task FillBirthDate(ChromiumWebBrowser browser, string birthDate)
        {
            // Doğum tarihini parse et (31.12.2005 formatından)
            if (!string.IsNullOrEmpty(birthDate))
            {
                await FillInputField(browser, "nx-input-2", birthDate);
            }
        }

        private async Task FillPlateInformation(ChromiumWebBrowser browser, KullaniciBilgileri info)
        {
            await Task.Delay(DefaultDelay);

            // Plaka numarasını doldur
            await FillInputField(browser, "inputPlateOne", info.txtPlakaNo);

            // Seri numarasını doldur
            await FillInputField(browser, "nx-input-3", info.txtSeriNo);
        }

        private async Task FillInputField(ChromiumWebBrowser browser, string elementId, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            var script = $@"
                var element = document.getElementById('{elementId}');
                if (element) {{
                    element.dispatchEvent(createEvent('focus'));
                    element.value = '{value}';
                    element.dispatchEvent(createEvent('change'));
                    element.dispatchEvent(createEvent('input'));
                    element.dispatchEvent(createEvent('blur'));
                }}";

            await browser.EvaluateScriptAsync(script);
        }

        private async Task WaitForVehicleInfo(ChromiumWebBrowser browser)
        {
            var vehicleInfo = await browser.EvaluateScriptAsync("document.getElementById('divVehicleRegistrationPlateInfo')?.innerText");
            while (vehicleInfo.Result == null)
            {
                await Task.Delay(200);
                vehicleInfo = await browser.EvaluateScriptAsync("document.getElementById('divVehicleRegistrationPlateInfo')?.innerText");
            }
        }

        private async Task SelectDistrict(ChromiumWebBrowser browser)
        {
            // İlçe dropdown'ını aç
            await browser.EvaluateScriptAsync("document.querySelector('#divRiskMotorAddressInformations nx-dropdown')?.click();");

            // İlk ilçeyi seç
            await browser.EvaluateScriptAsync(@"
                var ilceElement = document.querySelector(""[placeholder='İlçe Ara']"");
                if (ilceElement && ilceElement.parentElement && ilceElement.parentElement.parentElement) {
                    var firstOption = ilceElement.parentElement.parentElement.children[1]?.children[0];
                    if (firstOption) firstOption.click();
                }");

            await Task.Delay(2000);
        }

        private async Task<FiyatBilgisi> WaitForPriceResult(ChromiumWebBrowser browser)
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed.TotalSeconds < MaxWaitTimeSeconds)
            {
                try
                {
                    Thread.Sleep(2000);
                    var priceElement = await browser.EvaluateScriptAsync(@"document.querySelectorAll('.ng-untouched.ng-pristine')[2].value");

                    if (priceElement.Result != null && !string.IsNullOrEmpty(priceElement.Result.ToString()))
                    {
                        var price = priceElement.Result.ToString();

                        var commissionElement = await browser.EvaluateScriptAsync(@"
                            document.getElementsByClassName('calculate___container-body___price ng-star-inserted')[0]?.children[2]?.textContent");

                        var commission = commissionElement.Result?.ToString() ?? "";

                        return new FiyatBilgisi
                        {
                            BrutPrim = price,
                            Durum = "Başarılı",
                            FirmaAdi = InsuranceConstants.AllianzSigorta,
                            Komisyon = commission,
                            TeklifNo = GenerateTeklifNo(),
                        };
                    }

                    // Hata mesajı kontrolü
                    var errorMessage = await browser.EvaluateScriptAsync(@"
                        document.querySelector('.message-box .ng-star-inserted label')?.innerText");

                    if (errorMessage.Result != null && !string.IsNullOrEmpty(errorMessage.Result.ToString()))
                    {
                        return CreateErrorResult($"Sistem hatası: {errorMessage.Result}");
                    }
                }
                catch (Exception ex)
                {
                    // Bekleme sırasında oluşan hatalar için devam et
                    Console.WriteLine($"Fiyat sorgulanırken hata: {ex.Message}");
                }

                await Task.Delay(1000);
            }

            return CreateErrorResult("Zaman aşımı - Fiyat alınamadı");
        }

        private FiyatBilgisi CreateErrorResult(string errorMessage)
        {
            return new FiyatBilgisi
            {
                BrutPrim = "",
                Durum = errorMessage,
                FirmaAdi = InsuranceConstants.AllianzSigorta,
                Komisyon = "",
                TeklifNo = "",
            };
        }

        private string GenerateTeklifNo()
        {
            // Basit bir teklif numarası üretici
            return $"ALZ{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}