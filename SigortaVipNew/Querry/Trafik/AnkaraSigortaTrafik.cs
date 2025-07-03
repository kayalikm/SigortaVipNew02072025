using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Business;
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
    public class AnkaraSigortaTrafikFiyat : IFiyatSorgu
    {
        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            if (true)
            {
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

                // await browser.LoadUrlAsync("https://online.ankarasigorta.com.tr/");
                //Events
                
                await browser.EvaluateScriptAsync("var changeEvent=new Event(\"change\"),inputEvent=new Event(\"input\"),focusEvent=new Event(\"focus\"),blurEvent=new Event(\"blur\");");

                await WaitForPageLoad(browser);

                await browser.EvaluateScriptAsync(@"setTimeout(function() {
                   var links = document.querySelectorAll('a');
                    console.log('kod calisti');
                   for(var i = 0; i < links.length; i++) {
                       if(links[i].textContent.trim() === 'Trafik') {
                           links[i].click();
                           break;
                       }
                   }
                }, 1000);");
                string kullanimTarzi = "";
                await Task.Delay(2000);
                string kullanimTarzi1 = "";
                // Mehmet Abi bunları doldur 
                switch (info.txtkullanımtarzı)
                {
                    case "KAMYONET":
                        kullanimTarzi1 = "06";
                        break;
                    default:
                        kullanimTarzi1 = "01";
                        break;
                }

                // TC kimlik ve diğer bilgileri doldur TODO
                await browser.EvaluateScriptAsync($@"
                    var taxIdentityNumber = '{info.txtKimlikNo}';
                    var gsmNumber = '5436482556';
                    var cityName = 'İSTANBUL';
                    var dateOfBirth = '{ConvertDateFormat(info.txtDogumTar)}';
                    var plateNumber =  '{info.txtPlakaNo}';
                    var asbisNumber ='{info.txtSeriNo}';

                    function TaxIdentityNumberFunc() {{
                        console.info('TaxIdentityNumberFunc 1 ');
                        var taxIdentityObject = $('#InsuredCitizenshipOrTaxNumber');
                        setTimeout(function() {{
                            taxIdentityObject.val(taxIdentityNumber); 
                            taxIdentityObject.trigger('blur'); 
                            setTimeout('PlateFunc()', 2000);
                        }}, 500);
                    }}

                    function ClickCalculateButton() {{
                        console.info('ClickCalculateButton başladı');
                        
                        // Fiyat Hesapla butonunu bul ve tıkla
                        var calculateButton = $('button[type=submit]').filter(function() {{
                            return $(this).text().includes('Fiyat Hesapla');
                        }});
                        
                        if (calculateButton.length > 0) {{
                            calculateButton.click();
                            console.log('Fiyat Hesapla butonuna tıklandı');
                            
                            // Sonuç bekleme işlemini başlat
                       
                        }} else {{
                            console.log('Fiyat Hesapla butonu bulunamadı - alternatif yöntem denenecek');
                            
                            // Alternatif yöntem: class ve text ile ara
                            var altButton = $('.btn').filter(function() {{
                                return $(this).text().includes('Fiyat Hesapla');
                            }});
                            
                            if (altButton.length > 0) {{
                                altButton.click();
                                console.log('Alternatif yöntemle Fiyat Hesapla butonuna tıklandı');
                           
                            }} else {{
                                console.log('Hiçbir Fiyat Hesapla butonu bulunamadı');
                            }}
                        }}
                    }}

                    function PlateFunc() {{
                        console.info('PlateFunc başladı');
                        
                        // Önce 'Plaka ve ASBİS Henüz Yok' checkbox'ını işaretleme (false yap)
                        var hasNotPlateCheckbox = $('#HasNotPlate');
                        if (hasNotPlateCheckbox.length > 0) {{
                            hasNotPlateCheckbox.prop('checked', false).trigger('change');
                            console.log('HasNotPlate checkbox işaretlendi');
                        }}
                        
                        setTimeout(function() {{
                            // Plaka alanını doldur
                            var plateObject = $('#Plate');
                            if (plateObject.length > 0) {{
                                plateObject.val(plateNumber);
                                plateObject.trigger('change').trigger('blur');
                                console.log('Plaka dolduruldu: ' + plateNumber);
                            }}
                            
                            // Daha önce trafik poliçesi var mı - Hayır seç (IsRenewal_false)
                            var renewalNo = $('#IsRenewal_false');
                            if (renewalNo.length > 0) {{
                                renewalNo.prop('checked', true).trigger('change');
                                console.log('Yenileme Hayır seçildi');
                            }}
                            
                            // Kullanım Tarzı - HUSUSİ seç
                            var tariffCode = $('#TariffCode');
                            if (tariffCode.length > 0) {{
                                tariffCode.val('{kullanimTarzi1}').trigger('change');
                                console.log('Kullanım Tarzı HUSUSİ seçildi');
                            }}
                            
                            // ASBİS numarası doldur
                            var asbisObject = $('#AsbisNumber');
                            if (asbisObject.length > 0) {{
                                asbisObject.val(asbisNumber);
                                asbisObject.trigger('change').trigger('blur');
                                console.log('ASBİS numarası dolduruldu: ' + asbisNumber);
                                setTimeout('ClickCalculateButton()', 2000);
                            }}
                            
                            // Karavan mı - Hayır seç
                            var karavanNo = $('#KaravanMi_false');
                            if (karavanNo.length > 0) {{
                                karavanNo.prop('checked', true).trigger('change');
                                console.log('Karavan Hayır seçildi');
                            }}
                            
                            setTimeout('BirthDateFunc()', 1000);
                        }}, 1000);
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
                        if(typeof(console) != 'undefined') console.info('GsmNumberFunc ');
                        var intervalPageWaiting = setInterval(function() {{
                            console.log('GsmNumberFunc 1');
                            if(jQuery('.sk-spinner') != null && 
                               jQuery('.sk-spinner').css('display') == 'none') {{
                                
                                clearInterval(intervalPageWaiting);
                                if(jQuery('#SigortaliCepTelefonu') != null && jQuery('#SigortaliCepTelefonu').val() == '') {{
                                    var gsmObject = $('#SigortaliCepTelefonu');
                                    gsmObject.val(gsmNumber).trigger('change').trigger('blur');  
                                }}
                                if(jQuery('#SigortaliSirketTelefonu') != null && jQuery('#SigortaliSirketTelefonu').val() == '') {{
                                    var gsmObject1 = $('#SigortaliCepTelefonu');
                                    gsmObject1.val(gsmNumber).trigger('change').trigger('blur');  
                                }}
                            }}
                        }}, 1000);
                    }}

                    setTimeout('TaxIdentityNumberFunc()', 1000);
                ");

                await Task.Delay(15000); // Form doldurma işlemlerinin tamamlanması için daha uzun bekle

                var fiyat = await browser.EvaluateScriptAsync("$('#TrafikActPremium .form-control-static').first().text().trim()");
                var komisyon = "";

                var hata = await browser.EvaluateScriptAsync("document.getElementById(\"divHatalarIcerik\").innerText.trim()");

                if (fiyat.Result != null && fiyat.Result != "")
                    return new FiyatBilgisi
                    {
                        BrutPrim = fiyat.Result.ToString(),
                        Durum = "Tamamlandı",
                        FirmaAdi = InsuranceConstants.AnkaraSigorta,
                        Komisyon = komisyon,
                        TeklifNo = "",
                    };

                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = hata.Result != null ? hata.Result.ToString() : "Fiyat alınamadı",
                    FirmaAdi = InsuranceConstants.AnkaraSigorta,
                    Komisyon = "",
                    TeklifNo = "",
                };
            }
            return new FiyatBilgisi
            {
                BrutPrim = "",
                Durum = "İşlem tamamlanamadı",
                FirmaAdi = InsuranceConstants.AnkaraSigorta,
                Komisyon = "",
                TeklifNo = "",
            };
        }
         private async Task WaitForPageLoad(ChromiumWebBrowser browser)
        {
            int maxAttempts = 20;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
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

                await Task.Delay(500);
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

    }
}