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
    public class CorpusSigortaTrafikFiyat : IFiyatSorgu
    {
        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            try
            {
                // İptal kontrolü ve başlangıç progress
                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(5);

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

                progress?.Report(15);
                cancellationToken.ThrowIfCancellationRequested();

                await browser.LoadUrlAsync("https://sigorta.corpussigorta.com.tr/NonLife/Policy/SavePolicy.aspx?APP_MP=TR1");
                await Task.Delay(4000);

                progress?.Report(25);
                cancellationToken.ThrowIfCancellationRequested();

                // Tüm JavaScript kodunu çalıştır
                await browser.EvaluateScriptAsync("var changeEvent=new Event(\"change\"),inputEvent=new Event(\"input\"),focusEvent=new Event(\"focus\"),blurEvent=new Event(\"blur\");");
                await browser.EvaluateScriptAsync("var tc=document.getElementById(\"txtGIFTIdentityNo\");tc.dispatchEvent(focusEvent),tc.value=\"" + info.txtKimlikNo + "\",tc.dispatchEvent(changeEvent),tc.dispatchEvent(inputEvent),tc.dispatchEvent(blurEvent);");

                progress?.Report(35);
                cancellationToken.ThrowIfCancellationRequested();

                var loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");

                while (true)
                {
                    await Task.Delay(500);
                    cancellationToken.ThrowIfCancellationRequested();
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }

                progress?.Report(45);
                cancellationToken.ThrowIfCancellationRequested();
 

                //Plaka gir
                await browser.EvaluateScriptAsync("var plaka=document.getElementById(\"txtGIFTPlate\");plaka.dispatchEvent(focusEvent),plaka.value=\"" + info.txtPlakaNo + "\",plaka.dispatchEvent(changeEvent),plaka.dispatchEvent(inputEvent),plaka.dispatchEvent(blurEvent);");

                //Tescil kodu gir
                await browser.EvaluateScriptAsync("var tescilKod=document.getElementById(\"txtGIFTEGMSerial\");tescilKod.dispatchEvent(focusEvent),tescilKod.value=\"" + info.txtSeriNo.Substring(0, 2) + "\",tescilKod.dispatchEvent(changeEvent),tescilKod.dispatchEvent(inputEvent),tescilKod.dispatchEvent(blurEvent);");

                //Tescil no gir
                await browser.EvaluateScriptAsync("var tescil = document.getElementById(\"txtGIFTEGMNo\");\r\ntescil.dispatchEvent(focusEvent);\r\ntescil.value = \"" + info.txtSeriNo.Substring(2) + "\";\r\ntescil.dispatchEvent(changeEvent);\r\ntescil.dispatchEvent(inputEvent);\r\ntescil.dispatchEvent(blurEvent);");

                progress?.Report(55);
                cancellationToken.ThrowIfCancellationRequested();

                await browser.EvaluateScriptAsync("document.querySelectorAll(\".x-btn-text.icon-find \")[4].click();"); //Sorgula click

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }
 

                //Yeni tescil
                await browser.EvaluateScriptAsync("document.querySelectorAll(\".x-panel-btn-td button\")[12].click();");

                //Sigortalıdan taşı
                await browser.EvaluateScriptAsync("document.querySelector(\"#x-form-el-mfInsuredBy a\").click();");
                // İşlem tamamlanması için bekle

                string kullanimTarzi = info.txtkullanımtarzı;
                switch (info.txtkullanımtarzı)
                {
                    case "OTOMOBİL":
                        kullanimTarzi = "ÖZEL OTOMOBİL";
                        break;
                }

                var cboModelType = await browser.EvaluateScriptAsync("document.getElementById(\"cphCFB_cboModel\").value;");
                if (cboModelType.Result == "")
                {
                    //Kullanım tarzı seç
                    await browser.EvaluateScriptAsync("document.querySelector(\"#x-form-el-cphCFB_cboUsageType img\").click(); //liste click\r\nvar kullanimTarziListe = document.querySelectorAll(\".x-layer.x-combo-list\")[0].children[0].children;\r\nfor(let i = 0;i<kullanimTarziListe.length;i++){\r\n    if(kullanimTarziListe[i].innerHTML === \"" + kullanimTarzi + "\"){\r\n        kullanimTarziListe[i].click();\r\n        break;\r\n    }\r\n}");

                    while (true)
                    {
                        await Task.Delay(500);
                        loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                        if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                            break;
                    }
              

                    //Marka seç
                    await browser.EvaluateScriptAsync("document.querySelector(\"#x-form-el-cphCFB_cboMark img\").click();var markaListe=document.querySelectorAll(\".x-layer.x-combo-list\")[1].children[0].children;for(let i=0;i<markaListe.length;i++)if(\"" + info.txtMarka + "\"===markaListe[i].innerHTML){markaListe[i].click();break}");

                    while (true)
                    {
                        await Task.Delay(500);
                        loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                        if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                            break;
                    }
      

                    //Model yılı seç
                    await browser.EvaluateScriptAsync("document.querySelector(\"#x-form-el-cphCFB_cboModelYear img\").click(); //liste click\r\nvar modelYiliListe = document.querySelectorAll(\".x-layer.x-combo-list\")[2].children[0].children;\r\nfor(let i = 0;i<modelYiliListe.length;i++){\r\n    if(modelYiliListe[i].innerHTML === \"" + info.txtModel + "\"){\r\n        modelYiliListe[i].click();\r\n        break;\r\n    }\r\n}");

                    while (true)
                    {
                        await Task.Delay(500);
                        loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                        if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                            break;
                    }
                  

                    //Model seç
                    await browser.EvaluateScriptAsync("document.querySelector(\"#x-form-el-cphCFB_cboModel img\").click();\r\nvar modelListe = document.querySelectorAll(\".x-layer.x-combo-list\")[3]\r\n  .children[0].children;\r\nif (modelListe.length === 1) {\r\n  modelListe[0].click();\r\n} else {\r\n  for (let i = 0; i < modelListe.length; i++)\r\n    if (modelListe[i].innerHTML.split(\"|\")[1].trim() === \"" + info.txtAracKodu + "\") {\r\n      modelListe[i].click();\r\n      break;\r\n    }\r\n}\r\n");

                }

                await Task.Delay(500);

                await browser.EvaluateScriptAsync("document.querySelector(\".x-btn-text.icon-resultsetnext\").click();"); //Next click

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg\").children[0].innerHTML");
                    if (loader.Result == null) //Teminat loader kapandıysa (yükleme bittiyse)
                        break;
                }

                await Task.Delay(2500);
                if (info.txtTel == "")
                    info.txtTel = "5435467543";
                //Tel no gir
                await browser.EvaluateScriptAsync("var telNo=document.querySelectorAll(\"[id^='cphCFB_policyInputInformations_rptrInformations_numInformation_18'] input\")[0];telNo.dispatchEvent(focusEvent),telNo.value=\"" + info.txtTel + "\",telNo.dispatchEvent(changeEvent),telNo.dispatchEvent(inputEvent),telNo.dispatchEvent(blurEvent);");

                await Task.Delay(500);

                await browser.EvaluateScriptAsync("document.querySelector(\".x-btn-text.icon-resultsetnext\").click();"); //Next click

                await Task.Delay(20000); // Fiyat okuma için daha uzun bekleme

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }
          
                progress?.Report(85);
                cancellationToken.ThrowIfCancellationRequested();

                var hata = await browser.EvaluateScriptAsync("document.querySelector(\".ext-mb-content b\").innerText");
                var fiyat = await browser.EvaluateScriptAsync("document.querySelectorAll(\"#cphCFB_policyPaymentDetail_grdPremiumInfo .x-grid3-cell-inner.x-grid3-col-4\")[0].innerText;");
                var komisyon = await browser.EvaluateScriptAsync("document.querySelectorAll(\"#cphCFB_policyPaymentDetail_grdPremiumInfo .x-grid3-cell-inner.x-grid3-col-5\")[0].innerText;");

                progress?.Report(95);
                cancellationToken.ThrowIfCancellationRequested();

                if (fiyat.Result != null && fiyat.Result != "")
                {
                    progress?.Report(100);
                    return new FiyatBilgisi
                    {
                        BrutPrim = fiyat.Result.ToString(),
                        Durum = "Tamamlandı",
                        Komisyon = komisyon.Result.ToString(),
                        FirmaAdi = InsuranceConstants.CorpusSigorta,
                        TeklifNo = ""
                    };
                }

               

                
                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = "",
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
    }
}