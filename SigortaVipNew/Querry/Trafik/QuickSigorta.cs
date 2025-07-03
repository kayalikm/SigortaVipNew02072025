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
    public class QuickSigortaTrafikFiyat : IFiyatSorgu
    {
        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            if (true)
            {
                // İlk progress raporu
                progress?.Report(5);
                cancellationToken.ThrowIfCancellationRequested();

                var giris = await browser.EvaluateScriptAsync("document.querySelector(\"[value='login']\").innerText");
                if (giris.Result != null && giris.Result != "")
                    return new FiyatBilgisi
                    {
                        BrutPrim = "",
                        Durum = "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır",
                        FirmaAdi = InsuranceConstants.DogaSigorta,
                        Komisyon = "",
                        TeklifNo = "",
                    };

                progress?.Report(15);
                cancellationToken.ThrowIfCancellationRequested();
                string kullanimTarzi1 = "";
                // Mehmet Abi bunları doldur TODO
                switch (info.txtkullanımtarzı)
                {
                    case "KAMYONET":
                        kullanimTarzi1 = "Kamyonet";
                        break;
                    default:
                        kullanimTarzi1 = "Otomobil";
                        break;
                }

                // Sayfa yükleme
                await browser.LoadUrlAsync("https://portal.quicksigorta.com/uretim/trafik");
                await Task.Delay(5000, cancellationToken);

                // JavaScript değişkenlerini tanımla ve setNativeValue fonksiyonunu ekle
                string jsScript = $@"
                var identityNo = '{info.txtKimlikNo}';
                var birthDate = '{ConvertDateFormat(info.txtDogumTar)}';
                var plateYK = '{info.txtPlakaNo.Substring(0, 2)}';
                var plateNo = '{info.txtPlakaNo}';
                var modelYear = '{info.txtModel}';
                var engineNumberValue = '{ ""}';
                var chassisNumberValue = '{ ""}';
                var LicenseSerialLetter = '{info.txtSeriNo}';
                var LicenseSerialNumber = '{info.txtSeriNo}';
                var usageOtherStyle = '{kullanimTarzi1}';
                var brandCode = '{info.txtAracKodu.Substring(0, 3)}';
                var modelCode = '{info.txtAracKodu.Substring(3)}';
                var gsm = '5555555555';

                function setNativeValue(element, value) {{
                    if (!element) {{
                        console.error('Element not found.');
                        return;
                    }}

                    var valueSetter = Object.getOwnPropertyDescriptor(element, 'value').set;
                    var prototype = Object.getPrototypeOf(element);
                    var prototypeValueSetter = Object.getOwnPropertyDescriptor(prototype, 'value').set;

                    if (valueSetter && valueSetter !== prototypeValueSetter) {{
                        prototypeValueSetter.call(element, value);
                    }} else {{
                        valueSetter.call(element, value);
                    }}

                    element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                    element.dispatchEvent(new Event('change', {{ bubbles: true }}));
                }}

                function IndentityNoFunc() {{
                    if(typeof(console)!='undefined') console.info('IndentityNoFunc');
                    var IdentityNoInputField = document.querySelector(""[name='idNumber']"") || document.querySelector('#kimlikNoInput');
                    if(IdentityNoInputField) {{
                        setNativeValue(IdentityNoInputField, identityNo);
                        IdentityNoInputField.focus();
                        IdentityNoInputField.blur();
                    }}
                    setTimeout(function(){{ ConsumerBirthDateFunc(); }}, 2500);
                }}

                function ConsumerBirthDateFunc() {{
                    if(typeof(console)!='undefined') console.info('ConsumerBirthDateFunc');
                    var birthDayInputField = document.querySelector(""[name='birthDate']"") || document.querySelector('#dogumTarihiInput input');
                    
                    if(birthDayInputField) {{
                        setNativeValue(birthDayInputField, birthDate);
                        birthDayInputField.focus();
                        birthDayInputField.blur();
                        setTimeout(function(){{ PlateNoFunc(); }}, 2500);
                    }} else {{
                        setTimeout(function(){{ PlateNoFunc(); }}, 500);
                    }}
                }}

                function PlateNoFunc() {{
                    if(typeof(console)!='undefined') console.info('PlateNoFunc');
                    var plateInputField = document.querySelector(""[name='plateNumber']"") || document.querySelector('#plakaIlKoduInput');
                    var plateInputField2 = document.querySelector('#plakaKoduInput');
                    
                    if(plateInputField) {{
                        setNativeValue(plateInputField, plateNo);
                        plateInputField.focus();
                        plateInputField.blur();
                    }}
                    
                    if(plateInputField2) {{
                        setNativeValue(plateInputField2, plateNo.substring(2));
                        plateInputField2.focus();
                        plateInputField2.blur();
                    }}
                    
                    setTimeout(function(){{ SerialCodeFunc(); }}, 1000);
                }}

                function SerialCodeFunc() {{
                    if(typeof(console)!='undefined') console.info('SerialCodeFunc');
                    var serialCodeInputField = document.querySelector(""[name='serialCode']"") || document.querySelector('#tescilNoInput');
                    if(serialCodeInputField) {{
                        setNativeValue(serialCodeInputField, LicenseSerialLetter);
                        serialCodeInputField.focus();
                        serialCodeInputField.blur();
                    }}
                    setTimeout(function(){{ SerialNoFunc(); }}, 1000);
                }}

                function SerialNoFunc() {{
                    if(typeof(console)!='undefined') console.info('SerialNoFunc');
                    var serialnoInputField = document.querySelector(""[name='serialNo']"");
                    if(serialnoInputField) {{
                        setNativeValue(serialnoInputField, LicenseSerialNumber);
                        serialnoInputField.focus();
                        serialnoInputField.blur();
                    }}
                    setTimeout(function(){{ GSMFunc(); }}, 2500);
                }}

                function GSMFunc() {{
                    if(typeof(console)!='undefined') console.info('GSMFunc');
                    var gsmInputField = document.querySelector('#cepTelefonuInput');
                    if(gsmInputField) {{
                        setNativeValue(gsmInputField, gsm);
                        gsmInputField.focus();
                        gsmInputField.blur();
                    }}
                    setTimeout(function(){{ AracControlFunc(); }}, 1000);
                }}

                function AracControlFunc() {{
                    if(typeof(console)!='undefined') console.info('AracControlFunc');
                    var usageStyleInput = document.getElementById('usageStyle');
                    if (usageStyleInput) {{
                        setTimeout(function(){{ KullanimTarziFunc(); }}, 1500);
                    }} else {{
                        console.log('Usage style input elemanı bulunamadı.');
                        setTimeout(function(){{ MiniOnarimFunc(); }}, 1500);
                    }}
                }}

                function KullanimTarziFunc() {{
                    if(typeof(console)!='undefined') console.info('KullanimTarziFunc');
                    var usageStyleInput = document.getElementById('usageStyle');
                    if (usageStyleInput) {{
                        var popupButton = usageStyleInput.nextElementSibling.querySelector('.MuiAutocomplete-popupIndicator');
                        if (popupButton) {{
                            popupButton.click();
                            console.log('Usage style popup listesi açıldı.');
                            
                            setTimeout(function() {{
                                var options = document.querySelectorAll('.MuiAutocomplete-option');
                                for (let option of options) {{
                                    if (option.textContent.trim() === usageOtherStyle) {{
                                        option.click();
                                        console.log('Otomobil seçildi.');
                                        
                                        var inputEvent = new Event('input', {{ bubbles: true }});
                                        usageStyleInput.dispatchEvent(inputEvent);
                                        var changeEvent = new Event('change', {{ bubbles: true }});
                                        usageStyleInput.dispatchEvent(changeEvent);
                                        break;
                                    }}
                                }}
                                setTimeout(function(){{ BrandCodeFunc(); }}, 1500);
                            }}, 500);
                        }} else {{
                            console.log('Usage style popup butonu bulunamadı.');
                            setTimeout(function(){{ BrandCodeFunc(); }}, 1500);
                        }}
                    }} else {{
                        setTimeout(function(){{ BrandCodeFunc(); }}, 1500);
                    }}
                }}

                function BrandCodeFunc() {{
                    if(typeof(console)!='undefined') console.info('BrandCodeFunc');
                    var brandInput = document.getElementById('brand');
                    if (brandInput) {{
                        var popupButton = brandInput.nextElementSibling.querySelector('.MuiAutocomplete-popupIndicator');
                        if (popupButton) {{
                            popupButton.click();
                            console.log('Brand popup listesi açıldı.');
                            
                            setTimeout(function() {{
                                var options = document.querySelectorAll('.MuiAutocomplete-option');
                                for (let option of options) {{
                                    if (option.textContent.trim().startsWith(brandCode)) {{
                                        option.click();
                                        console.log('Brand seçildi: ' + option.textContent.trim());
                                        
                                        var inputEvent = new Event('input', {{ bubbles: true }});
                                        brandInput.dispatchEvent(inputEvent);
                                        var changeEvent = new Event('change', {{ bubbles: true }});
                                        brandInput.dispatchEvent(changeEvent);
                                        break;
                                    }}
                                }}
                                setTimeout(function(){{ ModelCodeFunc(); }}, 1500);
                            }}, 500);
                        }} else {{
                            console.log('Brand popup butonu bulunamadı.');
                            setTimeout(function(){{ ModelCodeFunc(); }}, 1500);
                        }}
                    }} else {{
                        setTimeout(function(){{ ModelCodeFunc(); }}, 1500);
                    }}
                }}

                function ModelCodeFunc() {{
                    if(typeof(console)!='undefined') console.info('ModelCodeFunc');
                    var modelInput = document.getElementById('model');
                    if (modelInput) {{
                        var popupButton = modelInput.nextElementSibling.querySelector('.MuiAutocomplete-popupIndicator');
                        if (popupButton) {{
                            popupButton.click();
                            console.log('Model popup listesi açıldı.');
                            
                            setTimeout(function() {{
                                var options = document.querySelectorAll('.MuiAutocomplete-option');
                                for (let option of options) {{
                                    if (option.textContent.trim().startsWith(modelCode)) {{
                                        option.click();
                                        console.log('Model seçildi: ' + option.textContent.trim());
                                        
                                        var inputEvent = new Event('input', {{ bubbles: true }});
                                        modelInput.dispatchEvent(inputEvent);
                                        var changeEvent = new Event('change', {{ bubbles: true }});
                                        modelInput.dispatchEvent(changeEvent);
                                        break;
                                    }}
                                }}
                                setTimeout(function(){{ MiniOnarimFunc(); }}, 1500);
                            }}, 500);
                        }} else {{
                            console.log('Model popup butonu bulunamadı.');
                            setTimeout(function(){{ MiniOnarimFunc(); }}, 1500);
                        }}
                    }} else {{
                        setTimeout(function(){{ MiniOnarimFunc(); }}, 1500);
                    }}
                }}

                function MiniOnarimFunc() {{
                    if(typeof(console)!='undefined') console.info('MiniOnarimFunc');
                    // Mini Onarım Servis seçimi
                    
                    
                   
                    setTimeout(function(){{ ContinueClick(); }}, 1000);
                }}

                function ekPaketFunc() {{
                    if(typeof(console)!='undefined') console.info('MiniOnarimFunc');
                    
                    document.querySelectorAll('input.PrivateSwitchBase-input.MuiSwitch-input.mui-1m9pwf3').forEach((checkbox) => {{
                        if (checkbox.checked) {{
                            checkbox.click();
                        }}
                    }});
                    
                    
                   
                    
                }}

                function ContinueClick() {{
                    if(typeof(console)!='undefined') console.info('ContinueClick');
                    var devamInputField = document.querySelector(""[type='submit']"") || document.querySelector('#hizliTrafikAnaDiv input[type=button]');
                    if(devamInputField) {{
                        devamInputField.click();
                        console.log('Hesapla butonuna tıklandı.');
                        setTimeout(function(){{ ekPaketFunc(); }}, 5000);
                    }} else {{
                        console.log('Hesapla butonu bulunamadı.');
                    }}
                }}

                // Başlangıç fonksiyonu
                setTimeout(function(){{ IndentityNoFunc(); }}, 1500);
                ";

                Console.WriteLine(jsScript);
                
                progress?.Report(30);
                cancellationToken.ThrowIfCancellationRequested();

                // JavaScript'i çalıştır
                await browser.EvaluateScriptAsync(jsScript);

                progress?.Report(50);
                cancellationToken.ThrowIfCancellationRequested();

                // Sonuçların yüklenmesini bekle
                await Task.Delay(10000, cancellationToken);

                progress?.Report(70);
                cancellationToken.ThrowIfCancellationRequested();

                // Loading kontrolü için bekle
                await WaitForPageLoad(browser, cancellationToken);

                progress?.Report(85);
                cancellationToken.ThrowIfCancellationRequested();

                // Sonuçları kontrol et
                var fiyatBilgisi = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var fiyatElement = document.getElementById('trafikSigortasiBundlePrice');
                        var teklifNoElement = document.getElementById('trafikSigortasiBundleProposal');
                        
                        var sonuc = {};
                        
                        if(fiyatElement) {
                            sonuc.fiyat = fiyatElement.textContent.trim();
                        }
                        
                        if(teklifNoElement) {
                            sonuc.teklifNo = teklifNoElement.textContent.trim();
                        }
                        
                        return JSON.stringify(sonuc);
                    })();
                ");

                // JavaScript sonucunu parse et
                if (fiyatBilgisi.Result != null && !string.IsNullOrEmpty(fiyatBilgisi.Result.ToString()))
                {
                    try
                    {
                        var jsonSonuc = fiyatBilgisi.Result.ToString();
                        var sonuc = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonSonuc);

                        if (sonuc != null && !string.IsNullOrEmpty(sonuc.fiyat?.ToString()))
                        {
                            progress?.Report(100);
                            
                            return new FiyatBilgisi
                            {
                                BrutPrim = sonuc.fiyat?.ToString() ?? "",
                                Durum = "Tamamlandı",
                                FirmaAdi = InsuranceConstants.QuickSigorta,
                                Komisyon = "",
                                TeklifNo = sonuc.teklifNo?.ToString() ?? ""
                            };
                        }
                    }
                    catch (Exception ex)
                    {
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
                        FirmaAdi = InsuranceConstants.QuickSigorta,
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
            int maxAttempts = 30;
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