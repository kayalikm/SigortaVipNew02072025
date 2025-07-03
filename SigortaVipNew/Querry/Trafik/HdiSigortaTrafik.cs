using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Constant;
using SigortaVip.FiyatSorgulamaFactory.Interface;
using SigortaVip.Models;
using SigortaVip.Models.Teminatlar;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVip.FiyatSorgulamaFactory.Concrete
{
    public class HdiSigortaTrafikFiyat : IFiyatSorgu
    {
        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            try
            {
                // Giriş kontrolü
                var giris = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var loginBtn = document.querySelector(""[value='login']"");
                        return loginBtn ? loginBtn.innerText : '';
                    })();
                ");
                if (!string.IsNullOrEmpty(giris.Result?.ToString()))
                {
                    return new FiyatBilgisi
                    {
                        BrutPrim = "",
                        Durum = "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır",
                        FirmaAdi = InsuranceConstants.SekerSigorta,
                        Komisyon = "",
                        TeklifNo = "",
                    };
                }

                await browser.LoadUrlAsync("https://portal.hdisigorta.com.tr/hdi-portal/spr/police_satis?execution=e22s1");
                await Task.Delay(4000);

                // Tüm JavaScript kodunu çalıştır
                //Events
                await browser.EvaluateScriptAsync("var changeEvent=new Event(\"change\"),inputEvent=new Event(\"input\"),focusEvent=new Event(\"focus\"),blurEvent=new Event(\"blur\");");

                //Tc veya Vergi no gir
                await browser.EvaluateScriptAsync("var tc=document.getElementById(\"policeSatisMusteriAramaForm:txtMusteriArama\");tc.dispatchEvent(focusEvent),tc.value=\"" + info.txtKimlikNo + "\",tc.dispatchEvent(changeEvent),tc.dispatchEvent(inputEvent),tc.dispatchEvent(blurEvent);");

                //Doğum Tarihi
                await browser.EvaluateScriptAsync("var dogumT=document.getElementById(\"policeSatisMusteriAramaForm:dateDogumTarihi_input\");dogumT.dispatchEvent(focusEvent),dogumT.value=\"" + info.txtDogumTar + "\",dogumT.dispatchEvent(changeEvent),dogumT.dispatchEvent(inputEvent),dogumT.dispatchEvent(blurEvent);");

                //Sorgula click
                await browser.EvaluateScriptAsync("document.getElementById(\"policeSatisMusteriAramaForm:commandButtonMusteriArama\").click();");

                var loader = await browser.EvaluateScriptAsync("document.getElementById(\"dialogPleaseWait\").getAttribute(\"aria-hidden\");");

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.getElementById(\"dialogPleaseWait\").getAttribute(\"aria-hidden\");");
                    if (loader.Result != null && loader.Result.ToString() == "true") //loader kapandıysa (yükleme bittiyse)
                        break;
                }

                await Task.Delay(1500);

                //Trafik click
                await browser.EvaluateScriptAsync("document.querySelectorAll(\".ui-g a.ui-commandlink.ui-widget\")[8].click();");

                // Plaka işlemi ekleniyor
                await Task.Delay(2000); // Trafik sayfasının yüklenmesi için bekle

                // Plaka ekleme JavaScript kodu
                await browser.EvaluateScriptAsync($@"
                    var plate = '{info.txtPlakaNo}'; 
                    
                    function PlateFunc() {{
                        if(typeof(console) != 'undefined') console.info('PlateFunc');
                        var plateObject = document.querySelector('[name*=txtPlaka]');
                        if(plateObject != null) {{
                            plateObject.value = plate;
                            plateObject.dispatchEvent(new Event('change'));
                            plateObject.dispatchEvent(new Event('input'));
                            setTimeout(function() {{
                                var submitBtn = document.querySelector('[type=""submit""]');
                                if(submitBtn) {{
                                    submitBtn.click();
                                }}
                            }}, 1000);
                        }}
                    }}
                    
                    setTimeout(function() {{ PlateFunc(); }}, 1000);
                ");

                // Araç bilgileri doldurma için bekle
                await Task.Delay(3000);

                // Detaylı araç bilgileri JavaScript kodu
                await browser.EvaluateScriptAsync($@"
                    var licenceSerialValue = '{info.txtSeriNo ?? "AA149189"}';
                    var vehicleBrandCodeValue = '153';
                    var modelYearValue = '{info.txtModel}';
                    var vehicleModelCodeValue = '{info.txtAracKodu}';
                    var engineNumberValue = '{"DCX052097"}';
                    var chassisNumberValue = '{"WVWZZZ3CZJE076349"}';
                    var vehicleStartDate = '{info.tescilTarihi ?? "01/01/2013"}';
                    var modelText = '{info.txtModel}';
                    var styleCodeNameValue = '{info.txtKullanimSekli ?? "HUSUSİ OTO"}';
                    var trafficDate = '{info.tescilTarihi ?? ""}';

                    function LicenceSerialFunc() {{
                        if(typeof(console)!='undefined') console.info('LicenceSerialFunc 1 ');
                        var licenceObject = $('[name*=txtTescilNo]');
                        if(licenceObject != null) {{
                            licenceObject[0].value = licenceSerialValue;
                            licenceObject.trigger('onchange');
                        }}

                        var intervalAlertWaiting=setInterval(function(){{
                            if($('#dialogPleaseWait') != null && $('#dialogPleaseWait').css('display') == 'none' ){{
                                clearInterval(intervalAlertWaiting);
                                setTimeout(function(){{ VehicleControl();}}, 1000);
                            }}
                        }},1000); 
                    }};

                    function VehicleControl() {{
                        if($('.selectlabel:contains(\'Motor No\')').length > 0 &&
                           $('.selectlabel:contains(\'Motor No\')') != null  &&
                           $('.selectlabel:contains(\'Motor No\')')[0].parentNode.children[0].innerText != '' )
                        {{ 
                            if(typeof(console)!='undefined') console.info('VehicleControl 1 '); 

                            if(styleCodeNameValue != 'HUSUSİ OTO'){{
                                setTimeout(function(){{ VehicleStyleFunc();}}, 1000);
                            }}else{{
                                setTimeout(function(){{ ImmSecimFunc();}}, 1000);
                            }} 
                        }}else {{
                            if(typeof(console)!='undefined') console.info('VehicleControl 2 ');
                            setTimeout(function(){{ VehicleBrandCodeFunc();}}, 1000);
                        }}
                    }};

                    function VehicleBrandCodeFunc() {{
                        if(typeof(console)!='undefined') console.info('VehicleBrandCodeFunc 1 ');
                        var brandObject = $('[name*=cmbMarka_input]')[0]; 
                        if (brandObject != null) {{
                            brandObject.value = vehicleBrandCodeValue;
                            brandObject.onchange();
                            var intervalAlertWaiting=setInterval(function(){{
                                if($('#dialogPleaseWait') != null && $('#dialogPleaseWait').css('display') == 'none' ){{
                                    clearInterval(intervalAlertWaiting);
                                    var valueText = $('[id*=cmbMarka]').find('option:contains(\'+ vehicleBrandCodeValue +\')').text();
                                    if(typeof(console)!='undefined') console.info('VehicleBrandCodeFunc valueText : ' + valueText); 
                                    $('[id*=cmbMarka_label]').html(valueText); 
                                    setTimeout(function(){{ VehicleModelYearCodeFunc();}}, 500);
                                }}
                            }},1000); 
                        }} 
                    }};

                    function VehicleModelYearCodeFunc() {{
                        if(typeof(console)!='undefined') console.info('VehicleModelYearCodeFunc 1 ');
                        var modelYearObject = $('[name*=cmbModelYili_input]')[0]; 
                        if (modelYearObject != null) {{
                            modelYearObject.value = modelYearValue;
                            modelYearObject.onchange();
                            var intervalAlertWaiting=setInterval(function(){{
                                if($('#dialogPleaseWait') != null && $('#dialogPleaseWait').css('display') == 'none' ){{
                                    clearInterval(intervalAlertWaiting);
                                    var valueText = $('[id*=cmbModelYili]').find('option:contains(\'+ modelYearValue +\')').text();
                                    if(typeof(console)!='undefined') console.info('VehicleModelYearCodeFunc valueText : ' + valueText); 
                                    $('[id*=cmbModelYili_label]').html(valueText); 
                                    setTimeout(function(){{ VehicleModelCodeFunc();}}, 500);
                                }}
                            }},1000); 
                        }} 
                    }};

                    function VehicleModelCodeFunc() {{
                        if(typeof(console)!='undefined') console.info('VehicleModelCodeFunc ');
                        var vehicleModelObject = $('[name*=cmbModel_input]')[0]; 
                        if (vehicleModelObject != null) {{
                            vehicleModelObject.value = vehicleModelCodeValue;
                            vehicleModelObject.onchange();
                            var intervalAlertWaiting=setInterval(function(){{
                                if($('#dialogPleaseWait') != null && $('#dialogPleaseWait').css('display') == 'none' ){{
                                    clearInterval(intervalAlertWaiting);
                                    var valueText = $('[id*=cmbModel]').find('option:contains(\'+ vehicleModelCodeValue +\')').text();
                                    if(typeof(console)!='undefined') console.info('VehicleModelCodeFunc valueText : ' + valueText); 
                                    $('[id*=cmbModel_label]').html(valueText); 

                                    if(styleCodeNameValue != 'HUSUSİ OTO'){{
                                        setTimeout(function(){{ VehicleStyleFunc();}}, 1000);
                                    }}else{{
                                        setTimeout(function(){{ EngineNumberFunc();}}, 1000);
                                    }}  
                                }}
                            }},1000); 
                        }} 
                    }};

                    function VehicleStyleFunc() {{
                        if(typeof(console)!='undefined') console.info('VehicleStyleFunc ');
                        var vehicleStyleObject = $('[name*=cmbKullanimTarzi_input]')[0]; 

                        if (vehicleStyleObject != null) {{
                            vehicleStyleObject.value =  $('[id*=cmbKullanimTarzi]').find('option:contains(\'+ styleCodeNameValue +\')').val();
                            vehicleStyleObject.onchange();
                            var intervalAlertWaiting2=setInterval(function(){{
                                if($('#dialogPleaseWait') != null && $('#dialogPleaseWait').css('display') == 'none' ){{
                                    clearInterval(intervalAlertWaiting2);
                                    var valueText = $('[id*=cmbKullanimTarzi]').find('option:contains(\'+ styleCodeNameValue +\')').text();
                                    if(typeof(console)!='undefined') console.info('VehicleModelCodeFunc valueText : ' + valueText); 
                                    $('[id*=cmbKullanimTarzi_label]').html(valueText); 
                                    setTimeout(function(){{ EngineNumberFunc();}}, 1000);
                                }}
                            }},1000); 
                        }}else {{
                            setTimeout(function(){{ EngineNumberFunc();}}, 1000);
                        }}
                    }};

                    function EngineNumberFunc() {{
                        if(typeof(console)!='undefined') console.info('EngineNumberFunc ');
                        var EngineObject = $('[name*=txtMotorNo]')[0]; 
                        if (EngineObject != null) {{
                            if(typeof(console)!='undefined') console.info('EngineNumberFunc 2  ');
                            EngineObject.value = engineNumberValue;
                        }} 
                        setTimeout(function(){{ ChassisNumberFunc();}}, 500);
                    }};

                    function ChassisNumberFunc() {{
                        if(typeof(console)!='undefined') console.info('ChassisNumberFunc 1  ');
                        var ChassisObject = $('[name*=txtSasiNo]')[0]; 
                        if (ChassisObject != null) {{
                            ChassisObject.value = chassisNumberValue;
                        }} 
                        setTimeout(function(){{ VehicleStartDateFunc();}}, 1000);
                    }};

                    function VehicleStartDateFunc() {{
                        if(typeof(console)!='undefined') console.info('VehicleStartDateFunc 1  ');
                        var vehicleStartObject = $('[name*=dateTescilTarihi]')[0]; 
                        if (vehicleStartObject != null && vehicleStartObject.value == '' ) {{
                            vehicleStartObject.value = trafficDate;
                        }} 
                        setTimeout(function(){{ ImmSecimFunc();}}, 500);
                    }};

                    function ImmSecimFunc() {{
                        if(typeof(console)!='undefined') console.info('ImmSecimFunc 1  ');
                        var ImmSecimObject = $('[name*=cmbTrafikImmSecim_input]'); 

                        if (ImmSecimObject != null) {{
                            ImmSecimObject[0].value =  'H';
                            $('[name*=cmbTrafikImmSecim').change();
                            var intervalAlertWaiting2=setInterval(function(){{
                                if($('#dialogPleaseWait') != null && $('#dialogPleaseWait').css('display') == 'none' ){{
                                    clearInterval(intervalAlertWaiting2);
                                    $('[id*=cmbTrafikImmSecim_label]').html('Hayır'); 
                                    setTimeout(function(){{ CalculateFunc();}}, 500);
                                }}
                            }},1000); 
                        }}else {{
                            setTimeout(function(){{ CalculateFunc();}}, 1000);
                        }}
                    }};

                    function CalculateFunc() {{
                        if(typeof(console)!='undefined') console.info('Calculate 1 ');
                        $('[id*=panelPrimHesaplama]').find('button').click();

                        var priceCheckInterval = setInterval(function() {{
                            if($('#dialogPleaseWait').css('display') == 'none') {{
                                clearInterval(priceCheckInterval);
                                
                                // Fiyat kontrolü için biraz bekle
                                setTimeout(function() {{
                                    var priceElement = $('[id*=alternatifPrim] td').first();
                                    if(priceElement.length > 0) {{
                                        var price = priceElement.text().trim();
                                        console.log('Fiyat bulundu:', price);
                                        
                                        var jsonData = {{
                                            'operation': 'Price Found Hdi',
                                            'message': 'Fiyat bulundu: ' + price,
                                            'price': price
                                        }};
                                        console.debug(JSON.stringify(jsonData));
                                    }} else {{
                                        console.log('Fiyat bulunamadı');
                                        var jsonData = {{
                                            'operation': 'Price Not Found Hdi',
                                            'message': 'Fiyat bulunamadı'
                                        }};
                                        console.debug(JSON.stringify(jsonData));
                                    }}
                                }}, 2000);
                            }}
                        }}, 1000);
                    }};

                    setTimeout(function(){{ LicenceSerialFunc();}}, 1000);
                ");

                // İşlem tamamlanması için bekle
                await Task.Delay(20000); // Tüm araç bilgileri işlemi için daha uzun bekleme

                // Fiyat sonuçlarını kontrol et
                var fiyatSonucu = await browser.EvaluateScriptAsync(@"
                    $('[id*=alternatifPrim] td').first().text().trim();
                ");

                // Console.debug mesajlarını da kontrol et (alternatif yöntem)
                var consoleLogs = await browser.EvaluateScriptAsync(@"
                    (function() {
                        if (window.lastConsoleLog && window.lastConsoleLog.includes('SekerSigorta_PriceResult')) {
                            return window.lastConsoleLog;
                        }
                        return '';
                    })();
                ");

                // Sonuç kontrolü
                var hata = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var hataDiv = document.querySelector('.x-window-body .error, .error-message, [class*=""error""]');
                        return hataDiv ? hataDiv.innerText : '';
                    })();
                ");

                var basarili = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var basariliDiv = document.querySelector('.x-window-body .success, .success-message, [class*=""success""]');
                        return basariliDiv ? basariliDiv.innerText : '';
                    })();
                ");

                // Fiyat sonucunu kontrol et
                string fiyatJsonStr = "";

                // İlk önce window.lastPriceResult'u kontrol et
                if (!string.IsNullOrEmpty(fiyatSonucu?.Result?.ToString()))
                {
                    fiyatJsonStr = fiyatSonucu.Result.ToString();
                }
                // Alternatif olarak console log'u kontrol et
                else if (!string.IsNullOrEmpty(consoleLogs?.Result?.ToString()))
                {
                    fiyatJsonStr = consoleLogs.Result.ToString();
                }

                if (!string.IsNullOrEmpty(fiyatJsonStr))
                {
                    try
                    {
                        // Newtonsoft.Json kullanarak parse et (System.Text.Json yerine)
                        dynamic fiyatData = fiyatJsonStr;

                        return new FiyatBilgisi
                        {
                            BrutPrim = fiyatData ?? "",
                            Komisyon ="",
                            Durum = "Başarılı",
                            FirmaAdi = InsuranceConstants.HdiSigorta,
                            TeklifNo = ""
                        };
                    }
                    catch (Exception jsonEx)
                    {
                        // JSON parse hatası durumunda hata mesajı ile birlikte döndür
                        return new FiyatBilgisi
                        {
                            BrutPrim = "",
                            Durum = $"JSON Parse Hatası: {jsonEx.Message} - Data: {fiyatJsonStr}",
                            FirmaAdi = InsuranceConstants.HdiSigorta,
                            Komisyon = "",
                            TeklifNo = ""
                        };
                    }
                }

                // Sonuç değerlendirmesi
                if (!string.IsNullOrEmpty(hata?.Result?.ToString()))
                {
                    return new FiyatBilgisi
                    {
                        BrutPrim = "",
                        Durum = hata.Result.ToString(),
                        FirmaAdi = InsuranceConstants.HdiSigorta,
                        Komisyon = "",
                        TeklifNo = ""
                    };
                }

                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = !string.IsNullOrEmpty(basarili?.Result?.ToString()) ?
                             "TRAMER ve Araç Bilgileri Sorgusu Tamamlandı" : "İşlem Tamamlandı",
                    FirmaAdi = InsuranceConstants.HdiSigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
            }
            catch (Exception ex)
            {
                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = $"Hata: {ex.Message}",
                    FirmaAdi = InsuranceConstants.HdiSigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
            }
        }
    }
}