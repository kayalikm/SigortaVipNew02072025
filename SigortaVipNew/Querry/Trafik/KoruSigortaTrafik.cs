using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Constant;
using SigortaVip.FiyatSorgulamaFactory.Interface;
using SigortaVip.Models;
using SigortaVip.Models.Teminatlar;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVip.FiyatSorgulamaFactory.Concrete
{
    public class KoruSigortaFiyat : IFiyatSorgu
    {



        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            if (true)
            {
                progress?.Report(5);

                var giris = await browser.EvaluateScriptAsync("document.querySelector(\"[value='login']\").innerText");
                if (giris.Result != null && giris.Result != "")
                    return new FiyatBilgisi
                    {
                        BrutPrim = "",
                        Durum = "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır",
                        FirmaAdi = InsuranceConstants.KoruSigorta,
                        Komisyon = "",
                        TeklifNo = "",
                    };

                progress?.Report(15);

                // Sayfa yükleme
                await browser.LoadUrlAsync("https://www.korusigortaportal.com/defaultV2.aspx#hizliTrafik");
                await Task.Delay(5000);

                // JavaScript değişkenlerini tanımla
                string jsScript = $@"
                 var identityNo = '{info.txtKimlikNo}';
                 var dateOfBirth = '{ConvertDateFormat(info.txtDogumTar)}';
                 var plate = '{info.txtPlakaNo.Substring(0, 2)}';
                 var plate2 = '{info.txtPlakaNo.Substring(2)}';
                 var gsm = '{"5555555555"}';
                 var serialNumber = '{info.txtSeriNo}';
                 var vehicleBrandCodeValue = '{info.txtAracKodu}';
                 var modelYearValue = '{info.txtModel}';
                 var vehicleModelCodeValue = '{""}';
                 var vehicleKindCodeValue = '{"01"}';
                 var vehicleUsageType = '{"1"}';
                 var engineNumberValue = '{""}';
                 var chassisNumberValue = '{""}';
                 var registrationDate = '';

                 setTimeout(function(){{ 
                     // Kimlik No
                     $('#kimlikNoInput').val(identityNo).trigger('change');
         
                     // Doğum Tarihi
                     var inputElement = document.querySelector('#dogumTarihiInput input');
                     if(inputElement) {{
                         inputElement.value = dateOfBirth;
                         var event = new Event('change', {{
                             bubbles: true,
                             cancelable: true,
                         }});
                         inputElement.dispatchEvent(event);
                     }}
         
                     // Plaka bilgileri
                     $('#plakaIlKoduInput').val(plate).trigger('change');
                     $('#plakaKoduInput').val(plate2).trigger('change');
         
                     // GSM
                     $('#cepTelefonuInput').val(gsm).trigger('change');
         
                     // Seçenek seçme fonksiyonu
                     function selectOptionByText(selectId, text) {{
                         var selectElement = $('#' + selectId);
                         selectElement.find('option').each(function() {{
                             if ($(this).text().trim() === text) {{
                                 $(this).prop('selected', true);
                                 selectElement.trigger('change');
                                 return false;
                             }}
                         }});
                     }}
         
                     setTimeout(function(){{ 
                         // Tescil No
                         if($('#tescilNoInput').val() == '') {{ 
                             $('#tescilNoInput').val(serialNumber).trigger('change'); 
                         }}
             
                         // Mini Onarım Servis seçimi
                         selectOptionByText('miniOnarimServis', 'RS'); 
             
                         // Hesapla butonuna tıkla
                         setTimeout(function(){{
                             var hesaplaButton = document.querySelector('#hizliTrafikAnaDiv input[type=button]');
                             if(hesaplaButton) {{
                                 hesaplaButton.click();
                             }}
                         }}, 1000);
                     }}, 1000);
                 }}, 1000);
             ";
                Console.WriteLine(jsScript);

                progress?.Report(25);

                // JavaScript'i çalıştır
                await Task.Delay(1000);
                await browser.EvaluateScriptAsync(jsScript);

                progress?.Report(50);

                // Loading kontrolü için bekle
                await WaitForPageLoad(browser, cancellationToken);

                progress?.Report(75);

                // Sonuçları kontrol et
                // burada en düşük fiyat ta bu tutara dahil edilecek TODO
                var sonucTablosu = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var tablo = document.getElementById('tblCaprazSatisTeklifTablosu');
                        var sonuclar = [];
                        
                        if(tablo) {
                            var satirlar = tablo.querySelectorAll('tbody tr');
                            
                            for(var i = 0; i < satirlar.length; i++) {
                                var satir = satirlar[i];
                                var teklifNoCell = satir.querySelector('[id*=td-pol-no]');
                                var urunAdiCell = satir.querySelector('[id*=td-urun-adi]');
                                var primCell = satir.querySelector('[id*=td-prim]');
                                
                                if(teklifNoCell && urunAdiCell && primCell) {
                                    var teklifNo = teklifNoCell.textContent.trim();
                                    var urunAdi = urunAdiCell.textContent.trim();
                                    var prim = primCell.textContent.trim();
                                    
                                    // Sadece TRAFIK ürünlerini al
                                    if(urunAdi.includes('TRAFIK')) {
                                        sonuclar.push({
                                            teklifNo: teklifNo,
                                            urunAdi: urunAdi,
                                            prim: prim
                                        });
                                    }
                                }
                            }
                        }
                        
                        return JSON.stringify(sonuclar);
                    })();
                ");

                // JavaScript sonucunu parse et
                if (sonucTablosu.Result != null && !string.IsNullOrEmpty(sonucTablosu.Result.ToString()))
                {
                    try
                    {
                        var jsonSonuc = sonucTablosu.Result.ToString();
                        var sonuclar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic[]>(jsonSonuc);

                        if (sonuclar != null && sonuclar.Length > 0)
                        {
                            // En uygun (ilk) teklifi al
                            var enUygunTeklif = sonuclar[0];

                            progress?.Report(100);

                            return new FiyatBilgisi
                            {
                                BrutPrim = enUygunTeklif.prim?.ToString() ?? "",
                                Durum = "Tamamlandı",
                                FirmaAdi = InsuranceConstants.KoruSigorta,
                                Komisyon = "", // Tabloda komisyon bilgisi yok
                                TeklifNo = enUygunTeklif.teklifNo?.ToString() ?? ""
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        // JSON parse hatası durumunda eski yöntemi dene
                        Console.WriteLine($"JSON Parse Hatası: {ex.Message}");
                    }
                }
                // Alternatif olarak eski yöntemi dene
                var hata = await browser.EvaluateScriptAsync("document.getElementById('divHatalarIcerik') ? document.getElementById('divHatalarIcerik').innerText : '';");
                var fiyat = await browser.EvaluateScriptAsync("document.querySelector(\"[id*='txtBrutPrim']\") ? document.querySelector(\"[id*='txtBrutPrim']\").value : '';");
                var komisyon = await browser.EvaluateScriptAsync("document.querySelector(\"[id*='txtKomisyon'][title='Komisyon'].AdaTextKapali\") ? document.querySelector(\"[id*='txtKomisyon'][title='Komisyon'].AdaTextKapali\").value : '';");

                // Alternatif fiyat kontrolü
                if (string.IsNullOrEmpty(fiyat?.Result?.ToString()))
                {
                    fiyat = await browser.EvaluateScriptAsync("document.querySelector('[id$=txtBrutPrim]') ? document.querySelector('[id$=txtBrutPrim]').value : '';");
                }

                if (string.IsNullOrEmpty(komisyon?.Result?.ToString()))
                {
                    komisyon = await browser.EvaluateScriptAsync("document.querySelector('[id$=txtKomisyon]') ? document.querySelector('[id$=txtKomisyon]').value : '';");
                }

                if (!string.IsNullOrEmpty(fiyat?.Result?.ToString()))
                {
                    progress?.Report(100);
                    
                    return new FiyatBilgisi
                    {
                        BrutPrim = fiyat.Result.ToString(),
                        Durum = "Tamamlandı",
                        FirmaAdi = InsuranceConstants.KoruSigorta,
                        Komisyon = komisyon?.Result?.ToString() ?? "",
                        TeklifNo = ""
                    };
                }

                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = !string.IsNullOrEmpty(hata?.Result?.ToString()) ? hata.Result.ToString() : "Fiyat alınamadı",
                    FirmaAdi = InsuranceConstants.KoruSigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
            }
        }



        private async Task WaitForPageLoad(ChromiumWebBrowser browser, CancellationToken cancellationToken = default)
        {
            int maxAttempts = 20;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                try
                {
                    var loader = await browser.EvaluateScriptAsync(@"
                        var divBekleme = document.getElementById('divBekleme');
                        if(divBekleme) {
                            var style = divBekleme.getAttribute('style');
                            if(style && style.includes('display')) {
                                return style.split(';').find(s => s.includes('display')).split(':')[1].trim();
                            }
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