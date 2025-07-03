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
    public class SekerSigortaTrafikFiyat : IFiyatSorgu
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

                await browser.LoadUrlAsync("https://eacente.sekersigorta.com.tr/NonLife/Policy/SavePolicy.aspx?APP_MP=310");
                await Task.Delay(4000);

                // Tüm JavaScript kodunu çalıştır
                await browser.EvaluateScriptAsync($@"
                    var plateBoxObject = txtPlateNo;
                    var kindCodeTypeObject = cboUsageType;
                    var brandCodeObject = cboMark;
                    var modelYearObject = cboModelYear;
                    var modelCodeObject = cboModel;
                    var identifyNoObject = txtGIFTIdentityNo;
                    var plateNoObject = txtGIFTPlate;
                    var egmSerialNoObject = txtGIFTEGMSerial;
                    var egmNoObject = txtGIFTEGMNo;
                    var sorgulaBtn = document.querySelector('#btnQuery button');
                    kindCode = '{info.txtKimlikNo}'
                    brandCode = '{info.txtSeriNo.Substring(0,2)}'
                    modelYear = '{info.txtModel}'
                    modelCode = '{info.txtAracKodu}'
                    var usageStyleValue = '001';

                    // Sigortalıdan Taşı butonuna tıkla
                    setTimeout(function() {{
                        try {{
                            $(""a:contains('Sigortalıdan Taşı')"")[0].click();
                        }} catch(e) {{
                            console.log('Sigortalıdan Taşı butonuna tıklanamadı:', e);
                        }}
                    }}, 1000);

                    function vehicleControl() {{
                        if (typeof console != 'undefined') console.info('vehicleControl');
                        var IntervalVehicleControl = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0) {{
                                if (kindCodeTypeObject.store.data.length >= 1) {{
                                    clearInterval(IntervalVehicleControl);
                                    
                                    if (modelCodeObject.getValue() == '' ||
                                        modelCodeObject.getRawValue().indexOf(modelCode) == -1) {{
                                        setTimeout(function () {{
                                            KindCodeTypeValueControl();
                                        }}, 1000);
                                    }} else {{
                                        setTimeout(function () {{
                                            AltKullanimTarziFunc();
                                        }}, 1000);
                                    }}
                                }}
                            }}
                        }}, 1000);
                    }}

                    function identifyControl() {{
                        if (typeof console != 'undefined') console.info('identifyControl');
                        var IntervalVehicleControl = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0) {{
                                if (identifyNoObject.disabled == false) {{
                                    clearInterval(IntervalVehicleControl);
                                    identifyNoObject.setValue('{info.txtKimlikNo}');
                                    identifyNoObject.fireEvent('blur');
                                    txtInsBirthdate.setValue('{ConvertDateFormat(info.txtDogumTar)}');
                                    txtInsBirthdate.fireEvent('blur');
                                }}
                                setTimeout(function () {{
                                    plateNoControl();
                                }}, 1000);
                            }}
                        }}, 1000);
                    }}

                    function plateNoControl() {{
                        if (typeof console != 'undefined') console.info('plateNoControl');
                        var IntervalVehicleControl = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0) {{
                                if (plateNoObject.disabled == false) {{
                                    clearInterval(IntervalVehicleControl);
                                    plateNoObject.setValue('{info.txtPlakaNo}');
                                    plateNoObject.fireEvent('blur');
                                }}
                                setTimeout(function () {{
                                    egmNoControl();
                                }}, 1000);
                            }}
                        }}, 1000);
                    }}

                    function egmNoControl() {{
                        if (typeof console != 'undefined') console.info('egmNoControl');
                        var IntervalVehicleControl = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0) {{
                                if (egmNoObject.disabled == false) {{
                                    clearInterval(IntervalVehicleControl);
                                    egmSerialNoObject.setValue('{(info.txtSeriNo?.Length >= 2 ? info.txtSeriNo.Substring(0, 2) : "")}');
                                    egmSerialNoObject.fireEvent('blur');
                                    egmNoObject.setValue('{(info.txtSeriNo?.Length > 2 ? info.txtSeriNo.Substring(2) : "")}');
                                    egmNoObject.fireEvent('blur');
                                    if (sorgulaBtn) {{
                                        sorgulaBtn.click();
                                    }}
                                }}
                                setTimeout(function () {{
                                    vehicleControl();
                                }}, 2000);
                            }}
                        }}, 1000);
                    }}

                    function KindCodeTypeValueControl() {{
                        if (typeof console != 'undefined') console.info('KindCodeTypeValueControl');
                        var IntervalKindCodeType = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0) {{
                                clearInterval(IntervalKindCodeType);
                                if (kindCodeTypeObject.disabled == false &&
                                    kindCodeTypeObject.store.data.length >= 1) {{
                                    kindCodeTypeObject.setValue(kindCode);
                                    kindCodeTypeObject.fireEvent('select');
                                }}
                                setTimeout(function () {{
                                    BrandCodeValueControl();
                                }}, 1000);
                            }}
                        }}, 1000);
                    }}

                    function BrandCodeValueControl() {{
                        if (typeof console != 'undefined') console.info('BrandCodeValueControl');
                        if (brandCode == '53') {{
                            brandCode = '053';
                        }}
                        var IntervalBrandCode = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0) {{
                                if (brandCodeObject.store.data.length >= 1) {{
                                    clearInterval(IntervalBrandCode);
                                    brandCodeObject.setValue(brandCode);
                                    brandCodeObject.fireEvent('select');
                                    setTimeout(function () {{
                                        ModelYearValueControl();
                                    }}, 1000);
                                }}
                            }}
                        }}, 1000);
                    }}

                    function ModelYearValueControl() {{
                        if (typeof console != 'undefined') console.info('ModelYearValueControl');
                        var IntervalModelYear = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0) {{
                                if (modelYearObject.store.data.length >= 1) {{
                                    clearInterval(IntervalModelYear);
                                    modelYearObject.setValue(modelYear);
                                    modelYearObject.fireEvent('select');
                                    setTimeout(function () {{
                                        ModelCodeValueControl();
                                    }}, 1000);
                                }}
                            }}
                        }}, 1000);
                    }}

                    function ModelCodeValueControl() {{
                        if (typeof console != 'undefined') console.info('ModelCodeValueControl');
                        var IntervalModelCode = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0) {{
                                if (modelCodeObject.store.data.length >= 1) {{
                                    clearInterval(IntervalModelCode);
                                    var modelLength = modelCodeObject.store.data.length;
                                    for (var i = 0; i < modelLength; i++) {{
                                        if (modelCodeObject.store.data.items[i].data.Value.indexOf(modelCode) != -1) {{
                                            if (typeof console != 'undefined')
                                                console.info('modelCode : ' + modelCode);

                                            var modelCodeFindValue = modelCodeObject.store.data.items[i].data.Key;
                                            modelCodeObject.setValue(modelCodeFindValue);
                                            modelCodeObject.fireEvent('select');
                                            if (typeof console != 'undefined')
                                                console.info('modelLength' + modelLength);
                                            setTimeout(function () {{
                                                AltKullanimTarziFunc();
                                            }}, 1000);
                                            break;
                                        }}
                                    }}
                                    setTimeout(function () {{
                                        if (modelCodeObject.getValue() == '') {{
                                            if (typeof console != 'undefined')
                                                console.info('Model Code Bulunamadı');
                                            var alerts = 'Tip kodu bulunamadı, Lütfen tip kodunu manuel seçiniz.';
                                            var jsonData = {{ operation: 'Vizyoneks Alerts', message: alerts }};
                                            console.debug(JSON.stringify(jsonData));
                                        }}
                                    }}, 1000);
                                }}
                            }}
                        }}, 1000);
                    }}

                    function AltKullanimTarziFunc() {{
                        if (typeof console != 'undefined') console.info('AltKullanimTarziFunc');
                        
                        // Alt Kullanım Tarzı için ExtJS objesini bul
                        var altKullanimTarziObject = null;
                        
                        // Önce label ile DOM elementini bul
                        var labels = $('.x-form-cb-label, .x-form-item-label');
                        for (var i = 0; i < labels.length; i++) {{
                            var labelText = labels[i].innerText || labels[i].textContent;
                            if (labelText.trim() == 'Alt Kullanim Tarzi:' || labelText.trim() == 'Alt Kullanım Tarzı:') {{
                                var forAttribute = labels[i].getAttribute('for');
                                if (forAttribute) {{
                                    try {{
                                        // ExtJS objesini eval ile al
                                        altKullanimTarziObject = eval(forAttribute);
                                        break;
                                    }} catch(e) {{
                                        console.error('Eval hatası:', e);
                                    }}
                                }}
                            }}
                        }}

                        if (altKullanimTarziObject != null && typeof altKullanimTarziObject.setValue === 'function') {{
                            altKullanimTarziObject.setValue(usageStyleValue);
                            if (typeof console != 'undefined') console.info('Alt Kullanım Tarzı değeri atandı: ' + usageStyleValue);
                        }} else {{
                            if (typeof console != 'undefined') console.warn('Alt Kullanım Tarzı ExtJS objesi bulunamadı');
                        }}
                        
                        setTimeout(function() {{ NextStepControl(); }}, 1500);
                    }}

                    function NextStepControl() {{
                        if (typeof console != 'undefined') console.info('NextStepControl');
                        var IntervalNextStepControl = setInterval(function () {{
                            if (typeof jQuery != 'undefined' &&
                                jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length == 0 &&
                                document.getElementById('btnNextAccordion') != null &&
                                typeof btnNextAccordion !== 'undefined' && btnNextAccordion.disabled == false) {{
                                clearInterval(IntervalNextStepControl);
                                $(""button:contains('Sonraki Adım')"").click();
                                
                                // Sonraki adıma geçince telefon kontrolünü başlat
                                setTimeout(function() {{
                                    phoneControlFunc();
                                }}, 2000);
                            }}
                        }}, 1000);
                    }}

                    // Telefon kontrol fonksiyonu
                    function phoneControlFunc() {{
                        if (typeof console != 'undefined') console.info('phoneControlFunc başlatıldı');

                        var phoneControlInterval = setInterval(function () {{
                            if (typeof console != 'undefined') console.info('Loading kontrol ediliyor...');

                            // jQuery kontrolü
                            if (typeof jQuery != 'undefined') {{
                                // Tüm loading mesajlarını kontrol et
                                var islemYapiliyor = jQuery("".x-mask-loading:contains('İşleminiz Yapılıyor ...')"").length > 0;
                                var teminatlarHesaplaniyor = jQuery("".x-mask-loading:contains('Teminatlar Hesaplanıyor ...')"").length > 0;
                                var tumLoadinglar = jQuery('.x-mask-loading').length > 0;

                                if (typeof console != 'undefined') {{
                                    console.info('İşleminiz Yapılıyor: ' + islemYapiliyor);
                                    console.info('Teminatlar Hesaplanıyor: ' + teminatlarHesaplaniyor);
                                    console.info('Tüm Loading Sayısı: ' + jQuery('.x-mask-loading').length);
                                }}

                                // Hiç loading yoksa ve telefon alanı var mı kontrol et
                                if (!tumLoadinglar) {{
                                    if (typeof console != 'undefined') console.info('Loading bitti, telefon alanı kontrol ediliyor');
                                    
                                    // Telefon alanının varlığını kontrol et
                                    var phoneExists = checkPhoneFieldExists();
                                    if (phoneExists) {{
                                        if (typeof console != 'undefined') console.info('Telefon alanı bulundu, doldurma başlıyor');
                                        clearInterval(phoneControlInterval);
                                        
                                        // Telefon numarasını ayarla
                                        setTimeout(function () {{
                                            setPhoneNumber();
                                        }}, 1000);
                                    }} else {{
                                        if (typeof console != 'undefined') console.info('Telefon alanı henüz yok, bekleniyor...');
                                    }}
                                }}
                            }}
                        }}, 2000); // Interval süresini artırdık
                    }}

                    function setPhoneNumber() {{
                        if (typeof console != 'undefined') console.info('setPhoneNumber çalışıyor');

                        var phoneValue = '{info.txtTel}';
                        var phoneObject = null;

                        // Farklı telefon alanlarını sırayla dene
                        var phoneSelectors = [
                            'cphCFB_policyInputInformations_rptrInformations_numInformation_19',
                            'input[placeholder*=""telefon""]',
                            'input[placeholder*=""Telefon""]',
                            'input[placeholder*=""phone""]',
                            'input[id*=""phone""]',
                            'input[id*=""telefon""]',
                            'input[name*=""phone""]',
                            'input[name*=""telefon""]'
                        ];

                        // ID'leri dene
                        for (var i = 0; i < phoneSelectors.length; i++) {{
                            if (phoneSelectors[i].indexOf('cph') === 0) {{
                                // ID ile ara
                                phoneObject = document.getElementById(phoneSelectors[i]);
                            }} else {{
                                // CSS selector ile ara
                                var elements = document.querySelectorAll(phoneSelectors[i]);
                                if (elements.length > 0) {{
                                    phoneObject = elements[0];
                                }}
                            }}
                            
                            if (phoneObject) {{
                                if (typeof console != 'undefined') console.info('Telefon alanı bulundu: ' + phoneSelectors[i]);
                                break;
                            }}
                        }}

                        // Eğer hala bulamazsa ExtJS objelerini kontrol et
                        if (!phoneObject) {{
                            try {{
                                // Tüm input alanlarını kontrol et
                                var allInputs = document.querySelectorAll('input[type=""text""], input.x-form-field');
                                for (var j = 0; j < allInputs.length; j++) {{
                                    var input = allInputs[j];
                                    var inputId = input.id || '';
                                    var inputName = input.name || '';
                                    var inputPlaceholder = input.placeholder || '';
                                    
                                    if (inputId.toLowerCase().includes('phone') || 
                                        inputId.toLowerCase().includes('telefon') ||
                                        inputName.toLowerCase().includes('phone') || 
                                        inputName.toLowerCase().includes('telefon') ||
                                        inputPlaceholder.toLowerCase().includes('phone') || 
                                        inputPlaceholder.toLowerCase().includes('telefon')) {{
                                        phoneObject = input;
                                        if (typeof console != 'undefined') console.info('Telefon alanı genel aramada bulundu: ' + inputId);
                                        break;
                                    }}
                                }}
                            }} catch(e) {{
                                if (typeof console != 'undefined') console.error('Genel arama hatası:', e);
                            }}
                        }}

                        if (phoneObject != null) {{
                            try {{
                                // ExtJS komponenti mi kontrol et
                                if (typeof phoneObject.setValue === 'function') {{
                                    phoneObject.setValue(phoneValue);
                                    phoneObject.fireEvent('blur');
                                    if (typeof console != 'undefined') console.info('ExtJS setValue kullanıldı');
                                }} else {{
                                    // Normal DOM elementi
                                    phoneObject.value = phoneValue;
                                    phoneObject.focus();

                                    // Çeşitli eventleri tetikle
                                    phoneObject.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                    phoneObject.dispatchEvent(new Event('change', {{ bubbles: true }}));
                                    phoneObject.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                                    phoneObject.dispatchEvent(new Event('keyup', {{ bubbles: true }}));

                                    // jQuery varsa
                                    if (typeof $ !== 'undefined') {{
                                        $(phoneObject).trigger('input').trigger('change').trigger('blur');
                                    }}

                                    if (typeof console != 'undefined') console.info('DOM value atama kullanıldı');
                                }}

                                if (typeof console != 'undefined') console.info('Telefon numarası başarıyla atandı: ' + phoneValue);

                                // Final NextStepControl'ü çağır
                                setTimeout(function () {{
                                    FinalNextStepControl();
                                }}, 2000);
                            }} catch (e) {{
                                if (typeof console != 'undefined') console.error('Telefon numarası atama hatası:', e);
                                // Hata olsa bile devam et
                                setTimeout(function () {{
                                    FinalNextStepControl();
                                }}, 2000);
                            }}
                        }} else {{
                            if (typeof console != 'undefined') console.error('Telefon input alanı bulunamadı');

                            // Tüm input alanlarını listele debug için
                            var debugInputs = document.querySelectorAll('input');
                            if (typeof console != 'undefined') console.info('Toplam input sayısı: ' + debugInputs.length);
                            for (var k = 0; k < Math.min(debugInputs.length, 10); k++) {{
                                var debugInput = debugInputs[k];
                                if (typeof console != 'undefined') console.info('Input ' + k + ': id=' + debugInput.id + ', name=' + debugInput.name + ', type=' + debugInput.type + ', placeholder=' + debugInput.placeholder);
                            }}

                            // Yine de FinalNextStepControl'ü çağır
                            setTimeout(function () {{
                                FinalNextStepControl();
                            }}, 2000);
                        }}
                    }}

                    // Telefon alanının varlığını kontrol eden fonksiyon
                    function checkPhoneFieldExists() {{
                        var phoneSelectors = [
                            'cphCFB_policyInputInformations_rptrInformations_numInformation_19',
                            'input[placeholder*=""telefon""]',
                            'input[placeholder*=""Telefon""]',
                            'input[id*=""phone""]',
                            'input[id*=""telefon""]'
                        ];

                        for (var i = 0; i < phoneSelectors.length; i++) {{
                            var element = null;
                            if (phoneSelectors[i].indexOf('cph') === 0) {{
                                element = document.getElementById(phoneSelectors[i]);
                            }} else {{
                                var elements = document.querySelectorAll(phoneSelectors[i]);
                                if (elements.length > 0) element = elements[0];
                            }}
                            
                            if (element && element.offsetParent !== null) {{ // visible check
                                return true;
                            }}
                        }}
                        return false;
                    }}

                    function getPhoneInput(labelText) {{
                        // Cep Tel için özel ID
                        if (labelText === 'Cep Tel') {{
                            var phoneInput = document.getElementById('cphCFB_policyInputInformations_rptrInformations_numInformation_19');
                            if (phoneInput) return phoneInput;
                        }}

                        // Label ile arama
                        var labels = document.querySelectorAll('.x-form-cb-label, .x-form-item-label, label');

                        for (var i = 0; i < labels.length; i++) {{
                            var labelTextContent = labels[i].textContent || labels[i].innerText;
                            if (labelTextContent && labelTextContent.trim() === labelText) {{
                                // Aynı satırda input ara
                                var row = labels[i].closest('tr');
                                if (row) {{
                                    var inputs = row.querySelectorAll('input[type=""text""], input.x-form-num-field');
                                    if (inputs.length > 0) {{
                                        return inputs[0];
                                    }}
                                }}

                                // for attribute varsa
                                var forAttr = labels[i].getAttribute('for');
                                if (forAttr) {{
                                    var element = document.getElementById(forAttr);
                                    if (element) return element;
                                }}
                            }}
                        }}

                        return null;
                    }}

                    // Final Next Step Control - telefon doldurulduktan sonra
                    function FinalNextStepControl() {{
                        if (typeof console != 'undefined') console.info('FinalNextStepControl başlatıldı');
                        
                        var finalControlInterval = setInterval(function () {{
                            if (typeof jQuery != 'undefined') {{
                                // Tüm loading'leri kontrol et
                                var tumLoadinglar = jQuery('.x-mask-loading').length;
                                var nextButton = document.getElementById('btnNextAccordion');
                                
                                if (typeof console != 'undefined') {{
                                    console.info('Loading sayısı: ' + tumLoadinglar);
                                    console.info('Next button var mı: ' + (nextButton !== null));
                                    if (nextButton) {{
                                        console.info('Next button disabled: ' + nextButton.disabled);
                                    }}
                                }}
                                
                                // Loading yoksa ve next button aktifse
                                if (tumLoadinglar == 0 && nextButton != null && !nextButton.disabled) {{
                                    clearInterval(finalControlInterval);
                                    
                                    // Birden fazla next button varsa en son bulunana tıkla
                                    var nextButtons = $(""button:contains('Sonraki Adım')"");
                                    if (nextButtons.length > 0) {{
                                        nextButtons.last().click();
                                        if (typeof console != 'undefined') console.info('Final Sonraki Adım tıklandı (' + nextButtons.length + ' button bulundu)');
                                         
                                        
                                        // Next butona tıkladıktan sonra fiyat tablosunu bekle
                                        setTimeout(function() {{
                                            waitForPriceTable();
                                        }}, 3000);
                                    }} else {{
                                        nextButton.click();
                                        if (typeof console != 'undefined') console.info('btnNextAccordion tıklandı');
                                        
                                        // Next butona tıkladıktan sonra fiyat tablosunu bekle
                                        setTimeout(function() {{
                                            waitForPriceTable();
                                        }}, 3000);
                                    }}
                                }} else if (nextButton != null && nextButton.disabled) {{
                                    if (typeof console != 'undefined') console.info('Next button disabled, bekleniyor...');
                                }}
                            }}
                        }}, 2000); // Interval süresini artırdık
                        
                        // 30 saniye sonra timeout
                        setTimeout(function() {{
                            clearInterval(finalControlInterval);
                            if (typeof console != 'undefined') console.warn('FinalNextStepControl timeout - işlem tamamlandı kabul ediliyor');
                        }}, 30000);
                    }}

                    // Fiyat tablosunu bekle ve oku
                    function waitForPriceTable() {{
                        if (typeof console != 'undefined') console.info('waitForPriceTable başlatıldı');
                        
                        var priceTableInterval = setInterval(function() {{
                            if (typeof jQuery != 'undefined') {{
                                // Loading kontrolü
                                var tumLoadinglar = jQuery('.x-mask-loading').length;
                                
                                if (tumLoadinglar == 0) {{
                                    // Evet butonuna tıkla ve bekle
                                    var buttonList = document.querySelectorAll('.x-btn-text');
                                    var evetClicked = false;
                                    for (let index = 0; index < buttonList.length; index++) {{
                                        if(buttonList[index].innerText == 'Evet') {{
                                            buttonList[index].click();
                                            evetClicked = true;
                                            console.log('Evet butonuna tıklandı');
                                            break;
                                        }}
                                    }}

                                    // Evet butonuna tıklandıysa, yeni bir interval başlat
                                    if (evetClicked) {{
                                        clearInterval(priceTableInterval);
                                        setTimeout(function() {{
                                            checkPriceTableAfterClick();
                                        }}, 3000); // 3 saniye bekle
                                    }}
                                }}
                            }}
                        }}, 2000);

                        // Yeni fiyat tablosu kontrol fonksiyonu
                        function checkPriceTableAfterClick() {{
                            var afterClickInterval = setInterval(function() {{
                                if (typeof jQuery != 'undefined') {{
                                    var tumLoadinglar = jQuery('.x-mask-loading').length;
                                    
                                    if (tumLoadinglar == 0) {{
                                        // Fiyat tablosunu ara
                                        var priceTable = findPriceTable();
                                        
                                        if (priceTable) {{
                                            if (typeof console != 'undefined') console.info('Fiyat tablosu bulundu, fiyatlar okunuyor');
                                            clearInterval(afterClickInterval);
                                            
                                            // Fiyatları oku ve konsola yazdır
                                            var priceData = extractPriceData(priceTable);
                                            
                                            // Sonuçları JSON formatında konsola yazdır
                                            var result = {{
                                                operation: 'SekerSigorta_PriceResult',
                                                brutPrim: priceData.brutPrim,
                                                komisyon: priceData.komisyon,
                                                netPrim: priceData.netPrim,
                                                vergi: priceData.vergi,
                                                durum: 'Başarılı',
                                                teklifNo: priceData.teklifNo || ''
                                            }};
                                            
                                            window.lastPriceResult = result;
                                            window.lastConsoleLog = JSON.stringify(result);
                                            console.debug(JSON.stringify(result));
                                        }}
                                    }}
                                }}
                            }}, 2000);

                            // 45 saniye timeout
                            setTimeout(function() {{
                                clearInterval(afterClickInterval);
                                if (typeof console != 'undefined') console.warn('Fiyat tablosu timeout');
                                
                                var timeoutResult = {{
                                    operation: 'SekerSigorta_PriceResult',
                                    brutPrim: '',
                                    komisyon: '',
                                    netPrim: '',
                                    vergi: '',
                                    durum: 'Fiyat tablosu bulunamadı',
                                    teklifNo: ''
                                }};
                                
                                window.lastPriceResult = timeoutResult;
                                window.lastConsoleLog = JSON.stringify(timeoutResult);
                                console.debug(JSON.stringify(timeoutResult));
                            }}, 45000);
                        }}
                        
                        // Ana interval için timeout
                        setTimeout(function() {{
                            clearInterval(priceTableInterval);
                            if (typeof console != 'undefined') console.warn('waitForPriceTable timeout');
                        }}, 60000);
                    }}

                    // Fiyat tablosunu bul
                    function findPriceTable() {{
                        // Çeşitli fiyat tablosu selector'ları dene
                        var tableSelectors = [
                            '.x-grid3-row-table',
                            'table.x-grid3-row-table',
                            '.x-grid-panel table',
                            '.x-grid3-body table',
                            'table[class*=""grid""]'
                        ];
                        
                        for (var i = 0; i < tableSelectors.length; i++) {{
                            var tables = document.querySelectorAll(tableSelectors[i]);
                            for (var j = 0; j < tables.length; j++) {{
                                var table = tables[j];
                                var tableText = table.textContent || table.innerText;
                                
                                // Fiyat bilgisi içeren tabloyu kontrol et
                                if (tableText.includes('Peşin') || 
                                    tableText.match(/\d{{1,3}}(?:,\d{{3}})*\.\d{{2}}/) ||
                                    tableText.includes('Brüt') ||
                                    tableText.includes('Komisyon')) {{
                                    
                                    if (typeof console != 'undefined') {{
                                        console.info('Fiyat tablosu bulundu: ' + tableSelectors[i]);
                                        console.info('Tablo içeriği: ' + tableText.substring(0, 200));
                                    }}
                                    return table;
                                }}
                            }}
                        }}
                        
                        return null;
                    }}

                    // Fiyat verilerini çıkar
                    function extractPriceData(table) {{
                        var priceData = {{
                            brutPrim: '',
                            komisyon: '',
                            netPrim: '',
                            vergi: '',
                            teklifNo: ''
                        }};
                        
                        try {{
                            // Tablodaki tüm hücreleri al
                            var cells = table.querySelectorAll('td .x-grid3-cell-inner');
                            
                            if (typeof console != 'undefined') {{
                                console.info('Toplam hücre sayısı: ' + cells.length);
                                for (var k = 0; k < cells.length; k++) {{
                                    console.info('Hücre ' + k + ': ' + cells[k].textContent);
                                }}
                            }}
                            
                            // Sayısal değerleri bul
                            var numbers = [];
                            for (var i = 0; i < cells.length; i++) {{
                                var cellText = cells[i].textContent.trim();
                                // Fiyat formatında olan değerleri bul (örn: 16,759.13)
                                if (cellText.match(/^\d{{1,3}}(?:,\d{{3}})*\.\d{{2}}$/)) {{
                                    numbers.push(cellText);
                                }}
                            }}
                            
                            if (typeof console != 'undefined') {{
                                console.info('Bulunan sayılar: ' + JSON.stringify(numbers));
                            }}
                            
                            // Sıralama: genellikle [BrutPrim, Komisyon, NetPrim, Vergi] şeklinde
                            if (numbers.length >= 4) {{
                                priceData.brutPrim = numbers[0];
                                priceData.komisyon = numbers[1];
                                priceData.netPrim = numbers[2];
                                priceData.vergi = numbers[3];
                            }} else if (numbers.length >= 2) {{
                                priceData.brutPrim = numbers[0];
                                priceData.komisyon = numbers[1];
                            }} else if (numbers.length >= 1) {{
                                priceData.brutPrim = numbers[0];
                            }}
                            
                            // Teklif numarasını ara
                            var allText = table.textContent || table.innerText;
                            var teklifMatch = allText.match(/(?:Teklif|Teklik).*?(\d{{8,}})/i);
                            if (teklifMatch) {{
                                priceData.teklifNo = teklifMatch[1];
                            }}
                            
                        }} catch(e) {{
                            if (typeof console != 'undefined') console.error('Fiyat çıkarma hatası:', e);
                        }}
                        
                        return priceData;
                    }}

                    var labels = 0;
                    var boxForName = '';
                    function getBoxForName(name) {{
                        if (typeof console != 'undefined') console.info('getBoxForName: ' + name);

                        // Tüm label'ları bul
                        var labels = $('.x-form-cb-label, .x-form-item-label');

                        if (typeof console != 'undefined')
                            console.info('Label sayısı: ' + labels.length);

                        for (var i = 0; i < labels.length; i++) {{
                            var labelText = labels[i].innerText || labels[i].textContent;
                            if (typeof console != 'undefined')
                                console.info('Label metni: ' + labelText);

                            if (labelText.trim() == name) {{
                                if (typeof console != 'undefined') console.info('Bulundu: ' + name);

                                // Label'ın for attribute'unu al
                                var forAttribute = labels[i].getAttribute('for');
                                if (typeof console != 'undefined')
                                    console.info('For attribute: ' + forAttribute);

                                // Eğer checkbox label'ı ise, ilgili input alanını bul
                                if (name === 'Cep Tel') {{
                                    // Cep Tel için özel durum - numInformation_19 alanını döndür
                                    var inputElement = document.getElementById(
                                        'cphCFB_policyInputInformations_rptrInformations_numInformation_19'
                                    );
                                    return inputElement;
                                }} else {{
                                    // Normal durumlar için
                                    var targetElement = document.getElementById(forAttribute);
                                    return targetElement;
                                }}
                            }}
                        }}
                        return null;
                    }}

                    // İşlemi başlat - önce TRAMER sorgusu, sonra araç bilgileri
                    setTimeout(function() {{
                        identifyControl();
                    }}, 500);

                    // Sürekli çalışan console.log'ları durdur
                    setTimeout(function() {{
                        try {{
                            // Sayfa üzerindeki sürekli interval'leri durdur
                            var highestTimeoutId = setTimeout(';');
                            for (var i = 0 ; i < highestTimeoutId ; i++) {{
                                clearTimeout(i); 
                            }}
                            
                            var highestIntervalId = setInterval(';');
                            for (var i = 0 ; i < highestIntervalId ; i++) {{
                                clearInterval(i); 
                            }}
                            
                            if (typeof console != 'undefined') console.info('Gereksiz interval/timeout\'lar temizlendi');
                        }} catch(e) {{
                            // Hata olursa görmezden gel
                        }}
                    }}, 95000); // 95 saniye sonra temizle
                ");

                // İşlem tamamlanması için bekle
                await Task.Delay(50000); // Fiyat okuma için daha uzun bekleme

                // Fiyat sonuçlarını kontrol et
                var fiyatSonucu = await browser.EvaluateScriptAsync(@"
                    (function() {
                        var result = null;
                        if (window.lastPriceResult) {
                            result = window.lastPriceResult;
                        }
                        return result ? JSON.stringify(result) : '';
                    })();
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
                        dynamic fiyatData = Newtonsoft.Json.JsonConvert.DeserializeObject(fiyatJsonStr);

                        return new FiyatBilgisi
                        {
                            BrutPrim = fiyatData?.brutPrim?.ToString() ?? "",
                            Komisyon = fiyatData?.komisyon?.ToString() ?? "",
                            Durum = fiyatData?.durum?.ToString() ?? "Başarılı",
                            FirmaAdi = InsuranceConstants.SekerSigorta,
                            TeklifNo = fiyatData?.teklifNo?.ToString() ?? ""
                        };
                    }
                    catch (Exception jsonEx)
                    {
                        // JSON parse hatası durumunda hata mesajı ile birlikte döndür
                        return new FiyatBilgisi
                        {
                            BrutPrim = "",
                            Durum = $"JSON Parse Hatası: {jsonEx.Message} - Data: {fiyatJsonStr}",
                            FirmaAdi = InsuranceConstants.SekerSigorta,
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
                        FirmaAdi = InsuranceConstants.SekerSigorta,
                        Komisyon = "",
                        TeklifNo = ""
                    };
                }

                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = !string.IsNullOrEmpty(basarili?.Result?.ToString()) ?
                             "TRAMER ve Araç Bilgileri Sorgusu Tamamlandı" : "İşlem Tamamlandı",
                    FirmaAdi = InsuranceConstants.SekerSigorta,
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
                    FirmaAdi = InsuranceConstants.SekerSigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
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