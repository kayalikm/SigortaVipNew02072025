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
    public class DogaSigorta2Fiyat : IFiyatSorgu
    {
    

        
        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            if (true)
            {
                // İptal kontrolü ve başlangıç progress
                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(5);

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

                await browser.LoadUrlAsync("https://portal.dogasigorta.com/default.aspx?p=urun~tumBranslar~polYonlendir&urun=310");

                //Events
                await browser.EvaluateScriptAsync("var focusEvent=new Event(\"focus\"),changeEvent=new Event(\"change\"),inputEvent=new Event(\"input\"),blurEvent=new Event(\"blur\");");

                progress?.Report(25);
                cancellationToken.ThrowIfCancellationRequested();

                //Tc
                var tc = info.txtKimlikNo;
                if (tc.Length == 11)
                {
                    await browser.EvaluateScriptAsync("var tcTextbox = document.getElementById(\"genelEkran_txtS_TcKimlikNo\");\r\ntcTextbox.dispatchEvent(focusEvent);\r\ntcTextbox.value = \"" + info.txtKimlikNo + "\";\r\ntcTextbox.dispatchEvent(changeEvent);\r\ntcTextbox.dispatchEvent(inputEvent);\r\ntcTextbox.dispatchEvent(blurEvent);");

                    progress?.Report(35);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                else
                {
                    await browser.EvaluateScriptAsync("var tcTextbox = document.getElementById(\"genelEkran_txtS_VergiNo\");\r\ntcTextbox.dispatchEvent(focusEvent);\r\ntcTextbox.value = \"" + info.txtKimlikNo + "\";\r\ntcTextbox.dispatchEvent(changeEvent);\r\ntcTextbox.dispatchEvent(inputEvent);\r\ntcTextbox.dispatchEvent(blurEvent);");
                    await Task.Delay(100);

                    // Adres Sol Button Click

                    await browser.EvaluateScriptAsync("document.getElementById('ImgM_IlSecim').click();");
                    await Task.Delay(2000);

                    var loader1 = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");

                    while (true)
                    {
                        await Task.Delay(500);
                        loader1 = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");
                        if (loader1.Result != null && loader1.Result.ToString() == "none") //loader kapandıysa (yükleme bittiyse)
                            break;
                    }

                    await browser.EvaluateScriptAsync($@"
                        var ilSelect = document.querySelector(""#ctl21_ctl00_lstIller"");
                        var il = """";
                        for(let i = 0; i < ilSelect.children.length; i++){{
                            if(ilSelect.children[i].value != """" && ""{info.txtIl}"" === ilSelect.children[i].label){{
                                il = ilSelect.children[i].value;
                                break;
                            }}
                        }}
                        ilSelect.dispatchEvent(focusEvent);
                        ilSelect.value = il;
                        ilSelect.dispatchEvent(changeEvent);
                        ilSelect.dispatchEvent(inputEvent);
                        ilSelect.dispatchEvent(blurEvent);");

                    await Task.Delay(2000);

                    await browser.EvaluateScriptAsync($@"
                        var ilceSelect = document.querySelector(""#ctl21_ctl00_lstIlceler"");
                        var ilce = """";
                        for(let i = 0; i < ilceSelect.children.length; i++){{
                            if(ilceSelect.children[i].value != """" && ilceSelect.children[i].label.includes(""{info.txtIlce}"") ){{
                                ilce = ilceSelect.children[i].value;
                                break;
                            }}
                        }}

                        ilceSelect.dispatchEvent(focusEvent);
                        ilceSelect.value = ilce;
                        ilceSelect.dispatchEvent(changeEvent);
                        ilceSelect.dispatchEvent(inputEvent);
                        ilceSelect.dispatchEvent(blurEvent);
                        ");

                    await Task.Delay(200);

                    await browser.EvaluateScriptAsync(@"
                        var tamamButtonList = document.querySelectorAll("".ui-button.ui-widget.ui-state-default.ui-corner-all.ui-button-text-only"")
                        for(var i = 0; i < tamamButtonList.length; i++){
                            if(tamamButtonList[i].innerText == ""Tamam""){
                                tamamButtonList[i].click();
                                break;
                            }
                        }");


                    await browser.EvaluateScriptAsync("document.getElementById('ImgS_IlSecim').click();");
                    await Task.Delay(2000);

                    var loader2 = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");

                    while (true)
                    {
                        await Task.Delay(500);
                        loader2 = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");
                        if (loader2.Result != null && loader2.Result.ToString() == "none") //loader kapandıysa (yükleme bittiyse)
                            break;
                    }

                    await browser.EvaluateScriptAsync($@"
                        var ilSelect = document.querySelector(""#ctl21_ctl00_lstIller"");
                        var il = """";
                        for(let i = 0; i < ilSelect.children.length; i++){{
                            if(ilSelect.children[i].value != """" && ""{info.txtIl}"" === ilSelect.children[i].label){{
                                il = ilSelect.children[i].value;
                                break;
                            }}
                        }}
                        ilSelect.dispatchEvent(focusEvent);
                        ilSelect.value = il;
                        ilSelect.dispatchEvent(changeEvent);
                        ilSelect.dispatchEvent(inputEvent);
                        ilSelect.dispatchEvent(blurEvent);");

                    await Task.Delay(2000);

                    await browser.EvaluateScriptAsync($@"
                        var ilceSelect = document.querySelector(""#ctl21_ctl00_lstIlceler"");
                        var ilce = """";
                        for(let i = 0; i < ilceSelect.children.length; i++){{
                            if(ilceSelect.children[i].value != """" && ilceSelect.children[i].label.includes(""{info.txtIlce}"") ){{
                                ilce = ilceSelect.children[i].value;
                                break;
                            }}
                        }}

                        ilceSelect.dispatchEvent(focusEvent);
                        ilceSelect.value = ilce;
                        ilceSelect.dispatchEvent(changeEvent);
                        ilceSelect.dispatchEvent(inputEvent);
                        ilceSelect.dispatchEvent(blurEvent);
                        ");

                    await Task.Delay(200);

                    await browser.EvaluateScriptAsync(@"
                        var tamamButtonList = document.querySelectorAll("".ui-button.ui-widget.ui-state-default.ui-corner-all.ui-button-text-only"")
                        for(var i = 0; i < tamamButtonList.length; i++){
                            if(tamamButtonList[i].innerText == ""Tamam""){
                                tamamButtonList[i].click();
                                break;
                            }
                        }");

                    await Task.Delay(1000);


                }
                var loader = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");

                while (true)
                {
                    await Task.Delay(500);
                    cancellationToken.ThrowIfCancellationRequested();
                    loader = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");
                    if (loader.Result != null && loader.Result.ToString() == "none") //loader kapandıysa (yükleme bittiyse)
                        break;
                }

                progress?.Report(50);
                cancellationToken.ThrowIfCancellationRequested();

               

                //Dogum Tarihi
                DateTime formattedDate;
                DateTime.TryParseExact(info.txtDogumTar, "dd/MM/yyyy", null, DateTimeStyles.None, out formattedDate);
                await browser.EvaluateScriptAsync("var dogumT=document.getElementById(\"tcSorgusuDogumTarihi\");dogumT.dispatchEvent(focusEvent),dogumT.value=\"" + formattedDate.ToString("yyyy'-'MM'-'dd") + "\",dogumT.dispatchEvent(changeEvent),dogumT.dispatchEvent(inputEvent),dogumT.dispatchEvent(blurEvent);");

                //Dogum tarihi ok click
                await browser.EvaluateScriptAsync("document.querySelector(\"#divDogumTarihiGirisDialog + div button\").click();");

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");
                    if (loader.Result != null && loader.Result.ToString() == "none") //loader kapandıysa (yükleme bittiyse)
                        break;
                }
              
                //Plaka kodu
                await browser.EvaluateScriptAsync("var plakaKodu=document.getElementById(\"trafikEkran_txtPlakaIlKodu\");plakaKodu.dispatchEvent(focusEvent),plakaKodu.value=\"" + info.txtPlakaNo.Substring(0, 2) + "\",plakaKodu.dispatchEvent(changeEvent),plakaKodu.dispatchEvent(inputEvent),plakaKodu.dispatchEvent(blurEvent);");

                //Plaka
                await browser.EvaluateScriptAsync("var plaka=document.getElementById(\"trafikEkran_txtPlaka\");plaka.dispatchEvent(focusEvent),plaka.value=\"" + info.txtPlakaNo.Substring(2) + "\",plaka.dispatchEvent(changeEvent),plaka.dispatchEvent(inputEvent),plaka.dispatchEvent(blurEvent);");

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");
                    if (loader.Result != null && loader.Result.ToString() == "none") //loader kapandıysa (yükleme bittiyse)
                        break;
                }
              
                //Uyarı click
                await browser.EvaluateScriptAsync("document.querySelector(\"#divPopupMesaj + div button\").click();");

                //Tescil kodu
                await browser.EvaluateScriptAsync("var tescilKod=document.getElementById(\"trafikEkran_txtTescilBelgeSeriKod\");tescilKod.dispatchEvent(focusEvent),tescilKod.value=\"" + info.txtSeriNo.Substring(0, 2) + "\",tescilKod.dispatchEvent(changeEvent),tescilKod.dispatchEvent(inputEvent),tescilKod.dispatchEvent(blurEvent);");

                //Tescil
                await browser.EvaluateScriptAsync("var tescilNo=document.getElementById(\"trafikEkran_txtTescilBelgeSeriNo\");tescilNo.dispatchEvent(focusEvent),tescilNo.value=\"" + info.txtSeriNo.Substring(2) + "\",tescilNo.dispatchEvent(changeEvent),tescilNo.dispatchEvent(inputEvent),tescilNo.dispatchEvent(blurEvent);");

                //Sorgula
                await browser.EvaluateScriptAsync("egmTramerSorgula();");

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.getElementById(\"divBeklemeJQ\").previousSibling.children[0].innerText");
                    if (loader.Result == null) //EGM loader kapandıysa (yükleme bittiyse)
                        break;
                }

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");
                    if (loader.Result != null && loader.Result.ToString() == "none") //loader kapandıysa (yükleme bittiyse)
                        break;
                }
              
                //Uyarı click
                await browser.EvaluateScriptAsync("document.querySelector(\"#divPopupMesaj + div button\").click();");

                //Araç kodu gir
                await browser.EvaluateScriptAsync("var aracKodu=document.getElementById(\"trafikEkran_txtAracKodu\");aracKodu.dispatchEvent(focusEvent),aracKodu.value=\"" + info.txtAracKodu + "\",aracKodu.dispatchEvent(changeEvent),aracKodu.dispatchEvent(inputEvent),aracKodu.dispatchEvent(blurEvent);");

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");
                    if (loader.Result != null && loader.Result.ToString() == "none") //loader kapandıysa (yükleme bittiyse)
                        break;
                }
             
                //Trafiğe çıkış
                await browser.EvaluateScriptAsync("var trafigeCikis=document.getElementById(\"trafikEkran_txtTrafigeCikisTarihi\");trafigeCikis.dispatchEvent(focusEvent),trafigeCikis.value=\"01-01-" + info.txtModel + "\",trafigeCikis.dispatchEvent(changeEvent),trafigeCikis.dispatchEvent(inputEvent),trafigeCikis.dispatchEvent(blurEvent);");

                //Hesapla click
                await browser.EvaluateScriptAsync("document.querySelector(\"[value='Hesapla']\").click();");

                while (true)
                {
                    await Task.Delay(500);
                    loader = await browser.EvaluateScriptAsync("document.getElementById(\"divBekleme\").getAttribute(\"style\").split(';')[4].trim().split(':')[1].trim();");
                    if (loader.Result != null && loader.Result.ToString() == "none") //loader kapandıysa (yükleme bittiyse)
                        break;
                }
              
                var fiyat = await browser.EvaluateScriptAsync("document.querySelector(\"[id*='txtBrutPrim']\").value");
                var komisyon = await browser.EvaluateScriptAsync("document.querySelector(\"[id*='txtKomisyon'][title='Komisyon'].AdaTextKapali\").value");
                var hata = await browser.EvaluateScriptAsync("document.getElementById(\"divHatalarIcerik\").innerText.trim()");

                if (fiyat.Result != null && fiyat.Result != "")
                    return new FiyatBilgisi
                    {
                        BrutPrim = fiyat.Result.ToString(),
                        Durum = "Tamamlandı",
                        FirmaAdi = InsuranceConstants.DogaSigorta,
                        Komisyon = komisyon.Result.ToString(),
                        TeklifNo = "",
                    };

                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = hata.Result != null ? hata.Result.ToString() : "Fiyat alınamadı",
                    FirmaAdi = InsuranceConstants.DogaSigorta,
                    Komisyon = "",
                    TeklifNo = "",
                };
            }
            return new FiyatBilgisi
            {
                BrutPrim = "",
                Durum = "İşlem tamamlanamadı",
                FirmaAdi = InsuranceConstants.DogaSigorta,
                Komisyon = "",
                TeklifNo = "",
            };

        }
    }
}
