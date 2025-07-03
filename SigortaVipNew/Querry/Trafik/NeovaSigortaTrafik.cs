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
    public class NeovaSigortaTrafikFiyat : IFiyatSorgu
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
                        FirmaAdi = InsuranceConstants.NeovaSigorta,
                        Komisyon = "",
                        TeklifNo = "",
                    };
                }

                progress?.Report(15);
                cancellationToken.ThrowIfCancellationRequested();

                await browser.LoadUrlAsync("https://sigorta.neova.com.tr:5443/NonLife/Policy/SavePolicy.aspx?APP_MP=TR4");
                await Task.Delay(4000);

                progress?.Report(25);
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(1000);

                //Events
                await browser.EvaluateScriptAsync("var changeEvent=new Event(\"change\"),inputEvent=new Event(\"input\"),focusEvent=new Event(\"focus\"),blurEvent=new Event(\"blur\");");

                var loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }
                cancellationToken.ThrowIfCancellationRequested();
                bool tc = true;
                progress?.Report(35);
                cancellationToken.ThrowIfCancellationRequested();

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


                switch (info.txtKimlikNo.Length)
                {
                    case 11:
                        //TC
                        await browser.EvaluateScriptAsync("var tc=document.getElementById(\"txtGIFTIdentityNo\");tc.dispatchEvent(focusEvent),tc.value=\"" + info.txtKimlikNo + "\",tc.dispatchEvent(changeEvent),tc.dispatchEvent(inputEvent),tc.dispatchEvent(blurEvent);");
                        tc = true;
                        break;
                    case 10:
                        await browser.EvaluateScriptAsync(@"var inputElement = document.getElementById('cboGIFTIdentityType');
                                                            var event = new MouseEvent('mousedown', {
                                                              bubbles: true,
                                                              cancelable: true,
                                                              view: window
                                                            });
                                                            inputElement.dispatchEvent(event);");

                        await Task.Delay(1300);

                        await browser.EvaluateScriptAsync(@"var divElements = document.querySelectorAll('.x-combo-list-item');
                                                                var secondElement = divElements[1];
                                                                
                                                                var mousedownEvent = new MouseEvent('mousedown', {
                                                                  bubbles: true,
                                                                  cancelable: true,
                                                                  view: window
                                                                });
                                                                secondElement.dispatchEvent(mousedownEvent);
                                                                
                                                                var focusEvent = new FocusEvent('focus', {
                                                                  bubbles: true,
                                                                  cancelable: true,
                                                                  view: window
                                                                });
                                                                secondElement.dispatchEvent(focusEvent);
                                                                
                                                                var mouseoverEvent = new MouseEvent('mouseover', {
                                                                  bubbles: true,
                                                                  cancelable: true,
                                                                  view: window
                                                                });
                                                                secondElement.dispatchEvent(mouseoverEvent);
                                                                
                                                                var clickEvent = new MouseEvent('click', {
                                                                  bubbles: true,
                                                                  cancelable: true,
                                                                  view: window
                                                                });
                                                                secondElement.dispatchEvent(clickEvent);
                                                                 ");

                        await browser.EvaluateScriptAsync("var tc=document.getElementById(\"txtGIFTIdentityNo\");tc.dispatchEvent(focusEvent),tc.value=\"" + info.txtKimlikNo + "\",tc.dispatchEvent(changeEvent),tc.dispatchEvent(inputEvent),tc.dispatchEvent(blurEvent);");

                        //await Task.Delay(1300);

                        //await browser.EvaluateScriptAsync("secondElement.click()");

                        //await browser.EvaluateScriptAsync("document.querySelectorAll(\"#x-form-el-cboGIFTIdentityType img\")[1].click(); //sel
                        tc = false;
                        break;
                }

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }
                cancellationToken.ThrowIfCancellationRequested();
                //Doğum Tarihi (Boşsa)
                await browser.EvaluateScriptAsync("var dogumT=document.getElementById(\"txtInsBirthdate\");\"\"===dogumT.value&&(dogumT.dispatchEvent(focusEvent),dogumT.value=\"" + info.txtDogumTar + "\",dogumT.dispatchEvent(changeEvent),dogumT.dispatchEvent(inputEvent),dogumT.dispatchEvent(blurEvent));");

                //Plaka
                await browser.EvaluateScriptAsync("var plaka=document.getElementById(\"txtGIFTPlate\");plaka.dispatchEvent(focusEvent),plaka.value=\"" + info.txtPlakaNo + "\",plaka.dispatchEvent(changeEvent),plaka.dispatchEvent(inputEvent),plaka.dispatchEvent(blurEvent);");

                //Tescil Kodu
                await browser.EvaluateScriptAsync("var tescilKod=document.getElementById(\"txtGIFTEGMSerial\");tescilKod.dispatchEvent(focusEvent),tescilKod.value=\"" + info.txtSeriNo.Substring(0, 2) + "\",tescilKod.dispatchEvent(changeEvent),tescilKod.dispatchEvent(inputEvent),tescilKod.dispatchEvent(blurEvent);");

                //Tescil
                await browser.EvaluateScriptAsync("var tescil=document.getElementById(\"txtGIFTEGMNo\");tescil.dispatchEvent(focusEvent),tescil.value=\"" + info.txtSeriNo.Substring(2) + "\",tescil.dispatchEvent(changeEvent),tescil.dispatchEvent(inputEvent),tescil.dispatchEvent(blurEvent);");

                //Sorgula click
                await browser.EvaluateScriptAsync("document.querySelectorAll(\".x-btn-text.icon-find \")[1].click();");
                //Next Click
                await browser.EvaluateScriptAsync("document.querySelector(\".x-btn-text.icon-resultsetnext\").click();");

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

                //Next Click
                await browser.EvaluateScriptAsync("document.querySelector(\".x-btn-text.icon-resultsetnext\").click();");
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(3000);



                //Next Click
                await browser.EvaluateScriptAsync("document.querySelector(\".x-btn-text.icon-resultsetnext\").click();");

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }
                //Tel No
                await Task.Delay(6000);
                if (info.txtTel == "")
                    info.txtTel = "5434565434";
                await browser.EvaluateScriptAsync("var telNo=document.getElementById(\"cphCFB_policyInputInformations_rptrInformations_numInformation_13\");telNo.dispatchEvent(focusEvent),telNo.value=\"" + info.txtTel + "\",telNo.dispatchEvent(changeEvent),telNo.dispatchEvent(inputEvent),telNo.dispatchEvent(blurEvent);");

                await Task.Delay(1000);
                cancellationToken.ThrowIfCancellationRequested();
                //Onay click
                await browser.EvaluateScriptAsync("document.querySelectorAll(\".x-window.x-window-plain.x-window-dlg button\")[0].click();");
                //Next Click
                await browser.EvaluateScriptAsync("document.querySelector(\".x-btn-text.icon-resultsetnext\").click();");
                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }
                await Task.Delay(1000);

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.querySelector(\".ext-el-mask-msg.x-mask-loading\").children[0].innerText");
                    if (loader.Result == null) //loader kapandıysa (yükleme bittiyse)
                        break;
                }
                await Task.Delay(1000);
                cancellationToken.ThrowIfCancellationRequested();

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
                        FirmaAdi = InsuranceConstants.NeovaSigorta,
                        TeklifNo = ""
                    };
                }




                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = "",
                    FirmaAdi = InsuranceConstants.NeovaSigorta,
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
                    FirmaAdi = InsuranceConstants.NeovaSigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
            }
        }
    }
}