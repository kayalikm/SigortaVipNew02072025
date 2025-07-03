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
    public class SompoJapanSigortaTrafik : IFiyatSorgu
    {
        public async Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            try
            {
                // İlk progress raporu
                progress?.Report(5);
                cancellationToken.ThrowIfCancellationRequested();

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
                        FirmaAdi = InsuranceConstants.SompoJapanSigorta,
                        Komisyon = "",
                        TeklifNo = "",
                    };
                }

                progress?.Report(15);
                cancellationToken.ThrowIfCancellationRequested();

                // Yeni İş sayfasına git
                await browser.LoadUrlAsync("https://cosmos.sompojapan.com.tr/?guid=cde11f5e-5014-4811-a7da-390e8a2abfeb&startupScript=52614B506A16E1209E52C33A2D21DEF5B761124842915AB2C7A856FF04781C07964708F4F864802E7C2C45060EBE983DFC99EBF3861094F0785961AD9170E8473812BCFE3B21EE1A1DA30065819F6D42903D992B8847BADA2AC6C78D6653CCD6F2FC7E435E5B7C517832CE42AB54EC8B1C49448E8E009ED8FC3ABC1DE3B5FDA2CAD419282A53AD595AB826AD059CD0B28FD1D356AE45C5642E4F26A425391DBEB9EC86A7248101AFDB008DF6F91C739EF63931CEF68BA05290C54B7E19E5B82AEE92540763D60FA0DF95098982253DEDDC7D452A47641FEE65CE86FAD237BE2E19FACC8FBFB2D2C4631B1530AD33292EF0C34C1BC0760649F5ABC8A068494E3A3C3227F1813D67F28C40A462121F651E21AE46B22A46CBECF24A82761AA7F61E78B2D841E18E089DD5E9B41907C2DF68F027855B6FA87CB5B894D1074578B549C58ECDF48203D9E521711038AD29AB7A0C89EFAE5673C433B9FE8D2FDCFF7642C0BF8624FA8C41DA85018D236D83CCC4F3740C8C78699EC365619610D06B505179AAD3AACA652477D1E2228CB3167B31D47840AD021C288516CA746EAD6B5E8832EAB25B8B8696D8AE13A881AA5B74B9AC8FA622ACEDA2153FBEC86DCE9AC2B07FE999521C08D467556434CAE3C26339155953B841FA2E8CBDD1662860CD8C0CC952A704E56CDE5A8678CE723A3F7826C3116778E928D3D0463A6D50AD009A82025096684DE5B526068970F4F42CE1C535055FF25310409F0982D84599E16C598715654E747BCEBD1C8974398AE9873E5C0745FE07FD8E42301F61944074CE848E33766B746CF3CD0F0465A14DFF19330B89E4567E6E5324DF505C9F58EAD354CB58579B3EF9A6711B2C52F9C8AA363B6274E9A7D5EB674F3A790C52B2FF2034357F6B1B39D4335C3330D6F091CE12DE");
                await Task.Delay(4000, cancellationToken);
                await browser.EvaluateScriptAsync("document.getElementById(\"cosmosYeniIs311\").click(); //trafiğe tıkla");

                var trafikCheck = await browser.EvaluateScriptAsync("document.querySelector(\"#chkTraffic\").checked");
                var kaskoCheck = await browser.EvaluateScriptAsync("document.querySelector(\"#chkCasco\").checked");

                while (true)
                {
                    trafikCheck = await browser.EvaluateScriptAsync("document.querySelector(\"#chkTraffic\").checked");
                    kaskoCheck = await browser.EvaluateScriptAsync("document.querySelector(\"#chkCasco\").checked");

                    if ((trafikCheck.Result != null && trafikCheck.Result != "") && (kaskoCheck.Result != null && kaskoCheck.Result != ""))
                    {
                        if ((Boolean)trafikCheck.Result && (Boolean)kaskoCheck.Result)
                            break;
                        else
                            continue;
                    }
                }

                await browser.EvaluateScriptAsync("document.getElementById('chkCasco').click();");
                bool tc = true;
                switch (info.txtKimlikNo.Length)
                {
                    case 11:
                        //Tc
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(2000);
                        await browser.EvaluateScriptAsync("let changeEvent=new Event(\"change\"),inputEvent=new Event(\"input\"),focusEvent=new Event(\"focus\"),blurEvent=new Event(\"blur\");document.getElementById(\"chkTraffic\").click();"); //trafik tickini kaldır

                        await browser.EvaluateScriptAsync("var tc=document.getElementById(\"txtIdentityOrTaxNo\");tc.dispatchEvent(focusEvent),tc.value=\"" + info.txtKimlikNo + "\",tc.dispatchEvent(changeEvent),tc.dispatchEvent(inputEvent),tc.dispatchEvent(blurEvent);"); //tc yaz

                        var dogumTText = await browser.EvaluateScriptAsync("document.getElementById(\"txtAllProductsBirthDate\").parentElement.children[1].innerHTML;");

                        while (true)
                        {
                            if (dogumTText.Result == null || dogumTText.Result == "")
                            {
                                dogumTText = await browser.EvaluateScriptAsync("document.getElementById(\"txtAllProductsBirthDate\").parentElement.children[1].innerHTML;");
                            }
                            else
                                break;
                        }
                        cancellationToken.ThrowIfCancellationRequested();

                        tc = true;
                        break;
                    case 10:
                        //Vergi no
                        await browser.EvaluateScriptAsync("document.getElementById('rblCustomerType_1').click();");
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(2000);
                        await browser.EvaluateScriptAsync("let changeEvent=new Event(\"change\"),inputEvent=new Event(\"input\"),focusEvent=new Event(\"focus\"),blurEvent=new Event(\"blur\");document.getElementById(\"chkCasco\").click();"); //trafik tickini kaldır

                        await browser.EvaluateScriptAsync("var vergi=document.getElementById(\"txtIdentityOrTaxNo\");vergi.dispatchEvent(focusEvent),vergi.value=\"" + info.txtKimlikNo + "\",vergi.dispatchEvent(changeEvent),vergi.dispatchEvent(inputEvent),vergi.dispatchEvent(blurEvent);"); //vergi yaz


                        cancellationToken.ThrowIfCancellationRequested();

                        tc = false;
                        break;
                }
                await browser.EvaluateScriptAsync("document.getElementById(\"rbVehicleDamaged_1\").click();");

                //Tel no gir
                await browser.EvaluateScriptAsync("var telKod=document.getElementById(\"txtInsuredGsmAreaCode\");telKod.dispatchEvent(focusEvent),telKod.value=\"" + info.txtTel.Substring(0, 3) + "\",telKod.dispatchEvent(changeEvent),telKod.dispatchEvent(inputEvent),telKod.dispatchEvent(blurEvent);var telNo=document.getElementById(\"txtInsuredGsmNumber\");telNo.dispatchEvent(focusEvent),telNo.value=\"" + info.txtTel.Substring(3) + "\",telNo.dispatchEvent(changeEvent),telNo.dispatchEvent(inputEvent),telNo.dispatchEvent(blurEvent);");

                //plaka gir
                await browser.EvaluateScriptAsync("var plakaKod=document.getElementById(\"txtPlateNoCityNo\");plakaKod.dispatchEvent(focusEvent),plakaKod.value=\"" + info.txtPlakaNo.Substring(0, 2) + "\",plakaKod.dispatchEvent(changeEvent),plakaKod.dispatchEvent(inputEvent),plakaKod.dispatchEvent(blurEvent);var plaka=document.getElementById(\"txtPlateNo\");plaka.dispatchEvent(focusEvent),plaka.value=\"" + info.txtPlakaNo.Substring(2) + "\",plaka.dispatchEvent(changeEvent),plaka.dispatchEvent(inputEvent),plaka.dispatchEvent(blurEvent);");

                await Task.Delay(2000);

                //tescil gir ve egm arat
                await browser.EvaluateScriptAsync("var tescilNo=document.getElementById(\"txtEGMNoNumber\");tescilNo.dispatchEvent(focusEvent),tescilNo.value=\"" + info.txtSeriNo.Substring(2) + "\",tescilNo.dispatchEvent(changeEvent),tescilNo.dispatchEvent(inputEvent),tescilNo.dispatchEvent(blurEvent),document.getElementById(\"btnSearchEgm\").click();");

                await Task.Delay(2000);
                await browser.EvaluateScriptAsync("var ce=new Event(\"change\"),ie=new Event(\"input\"),fe=new Event(\"focus\"),be=new Event(\"blur\"),kue=new Event(\"keyup\"),modelKodu=document.getElementById(\"txtModelCode\");modelKodu.dispatchEvent(fe),modelKodu.value=\"" + info.txtAracKodu + "\",modelKodu.dispatchEvent(ce),modelKodu.dispatchEvent(ie),modelKodu.dispatchEvent(be),modelKodu.dispatchEvent(kue);");
                await Task.Delay(2000);
                await browser.EvaluateScriptAsync("document.getElementById(\"btnProposalCreate\").click();");


                
                
                progress?.Report(25);
                cancellationToken.ThrowIfCancellationRequested();
                
                // Modal açılmasını bekle
                await Task.Delay(3000, cancellationToken);

                // Modal içinde Trafik kartını bekleyerek bul ve tıkla (Basitleştirilmiş Vue.js)
              
                progress?.Report(50);
                cancellationToken.ThrowIfCancellationRequested();

                // Trafik formunun yüklenmesini bekle
                await Task.Delay(3000, cancellationToken);

                progress?.Report(70);
                cancellationToken.ThrowIfCancellationRequested();

               
                progress?.Report(85);
                cancellationToken.ThrowIfCancellationRequested();

                // Sonuçları kontrol et
                await Task.Delay(4000, cancellationToken);

                var hata = await browser.EvaluateScriptAsync("document.getElementById(\"loadedDivTrafficProposalError\").children[0].innerHTML");
                var fiyat = await browser.EvaluateScriptAsync("document.getElementById(\"lblTrafficProposalGrossPremiumAlternative\").innerHTML");
                var komisyon = await browser.EvaluateScriptAsync("document.getElementById(\"lblTrafficProposalGrossPremium\").innerHTML");
                progress?.Report(100);

                if (fiyat.Result != null && !string.IsNullOrEmpty(fiyat.Result.ToString()))
                {
                    return new FiyatBilgisi
                    {
                        BrutPrim = fiyat.Result.ToString(),
                        Durum = "Tamamlandı",
                        Komisyon = komisyon.Result?.ToString() ?? "",
                        FirmaAdi = InsuranceConstants.SompoJapanSigorta,
                        TeklifNo = ""
                    };
                }

                // Hata mesajı varsa kontrol et
                if (hata.Result != null && !string.IsNullOrEmpty(hata.Result.ToString()))
                {
                    return new FiyatBilgisi
                    {
                        BrutPrim = "",
                        Durum = $"Hata: {hata.Result}",
                        FirmaAdi = InsuranceConstants.SompoJapanSigorta,
                        Komisyon = "",
                        TeklifNo = ""
                    };
                }

                return new FiyatBilgisi
                {
                    BrutPrim = "",
                    Durum = "Fiyat bilgisi alınamadı",
                    FirmaAdi = InsuranceConstants.SompoJapanSigorta,
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
                    FirmaAdi = InsuranceConstants.SompoJapanSigorta,
                    Komisyon = "",
                    TeklifNo = ""
                };
            }
        }
    }
}