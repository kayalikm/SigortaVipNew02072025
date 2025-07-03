using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Constant;
using SigortaVip.FiyatSorgulamaFactory.Interface;
using SigortaVip.Models;
using SigortaVip.Models.Teminatlar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVip.FiyatSorgulamaFactory.Concrete
{
    public class RaySigortaTrafikFiyat : IFiyatSorgu
    {
        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            try
            {
                // İlk progress raporu
                progress?.Report(5);
                cancellationToken.ThrowIfCancellationRequested();

                // Giriş kontrolü
                var giris = await browser.EvaluateScriptAsync("document.querySelector(\"[value='login']\").innerText");
                if (giris.Result != null && giris.Result != "")
                    return new FiyatBilgisi
                    {
                        BrutPrim = "",
                        Durum = "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır",
                        FirmaAdi = InsuranceConstants.RaySigorta,
                        Komisyon = "",
                        TeklifNo = "",
                    };

                progress?.Report(15);
                cancellationToken.ThrowIfCancellationRequested();

                // Ray Sigorta trafik sayfasına git
                await browser.LoadUrlAsync("https://rayexpress.raysigorta.com.tr/HizliTeklifIslemleri/Trafik");
                await Task.Delay(5000, cancellationToken);

                // JavaScript değişkenlerini tanımla
                string jsScript = $@"
                // PART 1: Vehicle Information
                var plateFirst = '{info.txtPlakaNo.Substring(0, 2)}';
                var vehicleBrandCode = '{info.txtAracKodu.Substring(0, 3)}';
                var vehicleModelCode = '{info.txtAracKodu.Substring(3)}';
                var vehicleKindName = 'OTOMOBİL';
                var plateSecond = '{info.txtPlakaNo.Substring(2)}';
                var licenseSerialNumber = '{info.txtSeriNo}';
                var engineNumber = '';
                var chassisNumber = '';
                var modelYearValue = '{info.txtModel}';

                // PART 2: Personal Information
                var taxIdentityNumber = '{info.txtKimlikNo}';
                var gsmNumber = '5525525455';
                var cityName = 'İSTANBUL';
                var dateOfBirth = '{ConvertDateToISO(info.txtDogumTar)}';

                function AlertModalControl() {{
                    console.info('AlertModalControl ediliyor');
                    if ($(""#TeklifAraModal"").css('display') == 'block') {{
                        console.info('AlertModal ekrani geldi. ve kapata basildi.');
                        setTimeout(function() {{
                            $("".col-lg-2 .btn-danger[type='button']"")[1].click();
                        }}, 1000);
                    }}
                    setTimeout('PlateFunc()', 1000);
                }}

                function PlateFunc() {{
                    console.info('PlateFunc 1 ');
                    var plateFirstObject = $(""[ng-model='PlakaIlKodu']"");
                    var plateFirstValue = $(""[ng-model='PlakaIlKodu'] option:contains('"" + plateFirst + ""')"").val();
                    setTimeout(function() {{
                        plateFirstObject.val(plateFirstValue).trigger('change');
                        var intervalPageWaiting = setInterval(function() {{
                            console.log('intervalPageWaiting 1');
                            if (jQuery('.sk-spinner').css('display') == 'none') {{
                                clearInterval(intervalPageWaiting);
                                setTimeout('PlateSecondFunc()', 1000);
                            }}
                        }}, 1000);
                    }}, 1000);
                }}

                function PlateSecondFunc() {{
                    console.info('PlateSecondFunc 1 ');
                    var plateSecondObject = $(""#Plaka"");
                    plateSecondObject.val(plateSecond).trigger('blur');
                    setTimeout(function() {{
                        var intervalPageWaiting = setInterval(function() {{
                            console.log('intervalPageWaiting 1 PlateSecondFunc ');
                            if (jQuery('.sk-spinner').css('display') == 'none') {{
                                clearInterval(intervalPageWaiting);
                                setTimeout('LicenceSerialFunc()', 2000);
                            }}
                        }}, 1000);
                    }}, 1000);
                }}

                function LicenceSerialFunc() {{
                    console.info('LicenceSerialFunc 1 ');
                    var tescilNumarasiObject = $(""#TescilNumarasi"");
                    tescilNumarasiObject.val(licenseSerialNumber).trigger('blur');

                    var intervalPageWaiting = setInterval(function() {{
                        console.log('intervalPageWaiting 1 LicenceSerialFunc');
                        if (jQuery('.sk-spinner').css('display') == 'none') {{
                            clearInterval(intervalPageWaiting);
                            setTimeout('AlertModalControl2()', 1000);
                        }}
                    }}, 1000);
                }}

                function AlertModalControl2() {{
                    var uyariObject = $(""#HizliTeklifMessageModal"");
                    if (uyariObject.length > 0 &&
                        uyariObject[0] != null &&
                        uyariObject[0].innerText != '') {{
                        var alerts = uyariObject[0].innerText;
                        if (alerts.indexOf('numaralı poliçenizin yenilemesi henüz gelmemiştir.') != -1) {{
                            if (typeof(console) != 'undefined') console.info('alerts : ' + alerts);
                            return;
                        }}
                    }}
                    setTimeout('BrandCodeFunc()', 1000);
                }}

                function BrandCodeFunc() {{
                    console.info('BrandCodeFunc 1 ');
                    var AracMarkaKodTextObject = $('#AracMarkaKodText');
                    if (AracMarkaKodTextObject != null && AracMarkaKodTextObject.val() == '') {{
                        AracMarkaKodTextObject.val(vehicleBrandCode).trigger('blur');
                        var intervalPageWaiting = setInterval(function() {{
                            console.log('intervalPageWaiting 1');
                            if (jQuery('.sk-spinner').css('display') == 'none') {{
                                clearInterval(intervalPageWaiting);
                                setTimeout('ModelCodeFunc()', 1000);
                            }}
                        }}, 1000);
                    }} else {{
                        setTimeout('ModelCodeFunc()', 1000);
                    }}
                }}

                function ModelCodeFunc() {{
                    console.info('ModelCodeFunc 1 ');
                    var AracMarkaTipKodTextObject = $('#AracMarkaTipKodText');
                    if (AracMarkaTipKodTextObject != null && AracMarkaTipKodTextObject.val() == '') {{
                        AracMarkaTipKodTextObject.val(vehicleModelCode).trigger('blur');
                        var intervalPageWaiting = setInterval(function() {{
                            console.log('intervalPageWaiting 1');
                            if (jQuery('.sk-spinner').css('display') == 'none') {{
                                clearInterval(intervalPageWaiting);
                                setTimeout('KullanimTarziFunc()', 1000);
                            }}
                        }}, 1000);
                    }} else {{
                        setTimeout('KullanimTarziFunc()', 1000);
                    }}
                }}

                function KullanimTarziFunc() {{
                    console.info('KullanimTarziFunc 1 ');
                    var vehicleKindNameObject = $(""[ng-model='KullanimTarzi']"");

                    //kamyon ile kamyonet farkı karışmasın diye filtre işlemi koyduk.
                    var vehicleKindNameValue = $(""[ng-model='KullanimTarzi'] option"").filter(function() {{
                        return $(this).text() === vehicleKindName;
                    }}).val();

                    if (vehicleKindNameValue == undefined) {{
                        vehicleKindNameValue = $(""[ng-model='KullanimTarzi'] option:contains('"" + vehicleKindName + ""')"").val();
                    }}

                    setTimeout(function() {{
                        if (vehicleKindNameValue != undefined) {{
                            vehicleKindNameObject.val(vehicleKindNameValue).trigger('change');
                        }}
                        var intervalPageWaiting = setInterval(function() {{
                            console.log('intervalPageWaiting 1');
                            if (jQuery('.sk-spinner').css('display') == 'none') {{
                                clearInterval(intervalPageWaiting);
                                setTimeout('CalculateFunc()', 3000);
                            }}
                        }}, 1000);
                    }}, 1000);
                }}

                function CalculateFunc() {{
                    console.info('CalculateFunc 1 ');
                    document.querySelector("".col-lg-2 .btn-success"").click();
                    setTimeout('TaxIdentityNumberFunc()', 3000);
                }}

                function TaxIdentityNumberFunc() {{
                    console.info('TaxIdentityNumberFunc 1 ');
                    var taxIdentityObject = $('#SigortaliTcknYkn');
                    setTimeout(function() {{
                        taxIdentityObject.val(taxIdentityNumber);
                        taxIdentityObject.trigger('blur');
                        setTimeout('BirthDateFunc()', 1000);
                    }}, 500);
                }}

                function BirthDateFunc() {{
                    console.info('BirthDateFunc 1 ');
                    var birthDayObject = $('#SigortaliDogumTarihi');
                    setTimeout(function() {{
                        birthDayObject.val(dateOfBirth);
                        birthDayObject.trigger('blur');
                        setTimeout('GsmNumberFunc()', 1000);
                    }}, 500);
                }}

                function GsmNumberFunc() {{
                    if (typeof(console) != 'undefined') console.info('GsmNumberFunc ')
                    var intervalPageWaiting = setInterval(function() {{
                        console.log('GsmNumberFunc 1');
                        if (jQuery('.sk-spinner') != null &&
                            jQuery('.sk-spinner').css('display') == 'none') {{
                            clearInterval(intervalPageWaiting);
                            if (jQuery('#SigortaliCepTelefonu') != null && jQuery('#SigortaliCepTelefonu').val() == '') {{
                                var gsmObject = $('#SigortaliCepTelefonu');
                                gsmObject.val(gsmNumber).trigger('change').trigger('blur');
                            }}
                            if (jQuery('#SigortaliSirketTelefonu') != null && jQuery('#SigortaliSirketTelefonu').val() == '') {{
                                var gsmObject1 = $('#SigortaliCepTelefonu');
                                gsmObject1.val(gsmNumber).trigger('change').trigger('blur');
                            }}
                        }}
                    }}, 1000);
                }}

                // Başlangıç fonksiyonu
                setTimeout('AlertModalControl()', 1000);
                ";

                Console.WriteLine(jsScript);
                
                progress?.Report(30);
                cancellationToken.ThrowIfCancellationRequested();

                // JavaScript'i çalıştır
                await browser.EvaluateScriptAsync(jsScript);

                progress?.Report(50);
                cancellationToken.ThrowIfCancellationRequested();

                // Sonuçların yüklenmesini bekle
                await Task.Delay(15000, cancellationToken);

                progress?.Report(70);
                cancellationToken.ThrowIfCancellationRequested();

                // Loading kontrolü için bekle
                await WaitForPageLoad(browser, cancellationToken);

                // Teklif listesi modalının açık olup olmadığını kontrol et
                var modalCheck = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var modal = document.querySelector('#TeklifAraModal');
                        return modal && modal.style.display === 'block';
                    })();
                ");

                if (modalCheck.Result != null && (bool)modalCheck.Result)
                {
                    // Modal kapatma butonuna tıkla
                    await browser.EvaluateScriptAsync(@"
                        var closeButton = document.querySelector('.col-lg-2 .btn-danger[type=""button""]');
                        if (closeButton) {
                            closeButton.click();
                        }
                    ");
                    await Task.Delay(2000);
                }

                // İlk olarak teklif listesinin varlığını kontrol et ve ilk teklifi seç
                var teklifSecmeIslemi = await browser.EvaluateScriptAsync(@"
                    (function() {
                        // Teklif tablosunu bul
                        var table = document.querySelector('#grupTanimTable');
                        if (!table) {
                            return JSON.stringify({
                                success: false,
                                message: 'Teklif tablosu bulunamadı',
                                action: 'table_not_found'
                            });
                        }
                        
                        // İlk teklif satırını bul (header'ı atla)
                        var dataRows = table.querySelectorAll('tr[ng-repeat]');
                        if (dataRows.length === 0) {
                            // ng-repeat yoksa normal tr'ları kontrol et
                            var allRows = table.querySelectorAll('tr');
                            dataRows = Array.from(allRows).slice(1); // İlk satır header
                        }
                        
                        if (dataRows.length > 0) {
                            var ilkSatir = dataRows[0];
                            var secButonu = ilkSatir.querySelector('button.btn-success');
                            
                            if (secButonu && secButonu.textContent.trim() === 'Seç') {
                                // İlk teklifi seç
                                secButonu.click();
                                
                                // Teklif bilgilerini çıkar
                                var cells = ilkSatir.querySelectorAll('td');
                                var teklifBilgisi = {
                                    policeNo: cells[0] ? cells[0].textContent.trim() : '',
                                    yenilemeNo: cells[1] ? cells[1].textContent.trim() : '',
                                    urunKodu: cells[2] ? cells[2].textContent.trim() : '',
                                    plaka: cells[3] ? cells[3].textContent.trim() : '',
                                    baslangicTarihi: cells[4] ? cells[4].textContent.trim() : '',
                                    bitisTarihi: cells[5] ? cells[5].textContent.trim() : '',
                                    prim: cells[6] ? cells[6].textContent.trim().replace(/[^0-9.,]/g, '') : ''
                                };
                                
                                return JSON.stringify({
                                    success: true,
                                    message: 'İlk teklif seçildi',
                                    action: 'selected',
                                    teklif: teklifBilgisi,
                                    toplamTeklifSayisi: dataRows.length
                                });
                            } else {
                                return JSON.stringify({
                                    success: false,
                                    message: 'Seç butonu bulunamadı',
                                    action: 'button_not_found'
                                });
                            }
                        } else {
                            return JSON.stringify({
                                success: false,
                                message: 'Teklif satırı bulunamadı',
                                action: 'no_rows'
                            });
                        }
                    })();
                ");

                progress?.Report(85);
                cancellationToken.ThrowIfCancellationRequested();

                // Seçim sonrasında sayfanın yüklenmesini bekle
                await Task.Delay(3000, cancellationToken);
                await WaitForPageLoad(browser, cancellationToken);

                progress?.Report(95);
                cancellationToken.ThrowIfCancellationRequested();

                // Teklif listesi işlem sonucunu kontrol et
                var teklifListesi = teklifSecmeIslemi;

                // Sonuçları işle
                if (teklifListesi.Result != null && !string.IsNullOrEmpty(teklifListesi.Result.ToString()))
                {
                    try
                    {
                        var jsonSonuc = teklifListesi.Result.ToString();
                        var sonuc = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonSonuc);

                        if (sonuc != null && sonuc.success == true && sonuc.action?.ToString() == "selected")
                        {
                            var teklif = sonuc.teklif;
                            
                            progress?.Report(100);

                            return new FiyatBilgisi
                            {
                                BrutPrim = teklif.prim?.ToString() ?? "",
                                Durum = $"Tamamlandı - İlk teklif seçildi ({sonuc.toplamTeklifSayisi} teklif arasından)",
                                FirmaAdi = InsuranceConstants.RaySigorta,
                                Komisyon = "",
                                TeklifNo = teklif.policeNo?.ToString() ?? ""
                            };
                        }
                        else if (sonuc != null && sonuc.success == false)
                        {
                            var errorMessage = sonuc.message?.ToString() ?? "Bilinmeyen hata";
                            Console.WriteLine($"Teklif seçme hatası: {errorMessage}");

                            // Hata durumunda alternatif çözüm dene
                            return await AlternatifTeklifOkuma(browser);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Teklif seçme JSON Parse Hatası: {ex.Message}");
                        return await AlternatifTeklifOkuma(browser);
                    }
                }

                // Başarısız olursa alternatif yöntemi dene
                return await AlternatifTeklifOkuma(browser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ray Sigorta Hata: {ex.Message}");
                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = $"Hata: {ex.Message}",
                    FirmaAdi = InsuranceConstants.RaySigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
            }
        }

        private async Task<FiyatBilgisi> AlternatifTeklifOkuma(ChromiumWebBrowser browser)
        {
            try
            {
                // Manuel teklif seçimi deneme
                var manuelSecim = await browser.EvaluateScriptAsync(@"
                    (function() {
                        // İlk Seç butonunu bul ve tıkla
                        var buttons = document.querySelectorAll('button.btn-success');
                        for (var i = 0; i < buttons.length; i++) {
                            if (buttons[i].textContent.trim() === 'Seç') {
                                buttons[i].click();
                                return 'İlk Seç butonuna tıklandı';
                            }
                        }
                        
                        // Alternatif: ng-click ile seçim
                        var ngClickButtons = document.querySelectorAll('[ng-click*=""TeklifSec""]');
                        if (ngClickButtons.length > 0) {
                            ngClickButtons[0].click();
                            return 'ng-click ile seçim yapıldı';
                        }
                        
                        return 'Seç butonu bulunamadı';
                    })();
                ");

                await Task.Delay(3000);
                await WaitForPageLoad(browser, CancellationToken.None);

                // Sonuç sayfasından fiyat bilgisini al
                var sonucFiyat = await browser.EvaluateScriptAsync(@"
                    document.getElementById('TeklifBrutPrim').innerText;
                ");

                if (!string.IsNullOrEmpty(sonucFiyat?.Result?.ToString()))
                {
                    return new FiyatBilgisi
                    {
                        BrutPrim = sonucFiyat.Result.ToString(),
                        Durum = "Tamamlandı - Alternatif yöntemle seçildi",
                        FirmaAdi = InsuranceConstants.RaySigorta,
                        Komisyon = "",
                        TeklifNo = ""
                    };
                }

                // Son çare: hata mesajını kontrol et
                var hata = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var errorSelectors = [
                            '.error-message', '.alert-danger', '.error', 
                            '[class*=""error""]', '[class*=""uyari""]', 
                            '#HizliTeklifMessageModal'
                        ];
                        
                        for (var i = 0; i < errorSelectors.length; i++) {
                            var element = document.querySelector(errorSelectors[i]);
                            if (element && element.textContent.trim()) {
                                return element.textContent.trim();
                            }
                        }
                        return '';
                    })();
                ");

                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = !string.IsNullOrEmpty(hata?.Result?.ToString()) ?
                           hata.Result.ToString() : "Teklif seçilemedi - Manuel işlem gerekli",
                    FirmaAdi = InsuranceConstants.RaySigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
            }
            catch (Exception ex)
            {
                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = $"Alternatif okuma hatası: {ex.Message}",
                    FirmaAdi = InsuranceConstants.RaySigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
            }
        }

        private async Task WaitForPageLoad(ChromiumWebBrowser browser, CancellationToken cancellationToken = default)
        {
            int maxAttempts = 30;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                try
                {
                    var loader = await browser.EvaluateScriptAsync(@"
                        var spinner = jQuery('.sk-spinner');
                        if(spinner && spinner.length > 0) {
                            return spinner.css('display');
                        }
                        return 'none';
                    ");

                    if (loader.Result != null && loader.Result.ToString() == "none")
                        break;
                }
                catch
                {
                    // Loader elementi bulunamazsa devam et
                }

                await Task.Delay(500, cancellationToken);
                attempts++;
            }
        }

        private string ConvertDateFormat(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return "";

            try
            {
                // dd/MM/yyyy formatından dd.MM.yyyy formatına çevir
                if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime date))
                {
                    return date.ToString("dd.MM.yyyy");
                }
                return dateStr;
            }
            catch
            {
                return dateStr;
            }
        }

        private string ConvertDateToISO(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return "";

            try
            {
                if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime date))
                {
                    return date.ToString("yyyy-MM-dd");
                }
                return dateStr;
            }
            catch
            {
                return dateStr;
            }
        }
    }
}