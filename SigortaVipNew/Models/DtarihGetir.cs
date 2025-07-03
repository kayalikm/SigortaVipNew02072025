using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Business;
using SigortaVip.Constant;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVip.Models
{
    internal class DtarihGetir
    {
        public async Task<string> DtarihGetirAsync(string Firmaİsmi,string tcKimlikNo)
        {
            switch (Firmaİsmi)
            {
                case Constant.InsuranceConstants.BereketSigorta:
                    ChromiumWebBrowser BereketSigortaBrowser;
                    if (!Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.BereketSigorta).Any())
                    {
                        return "Şirket Kapalı";
                    }
                    BereketSigortaBrowser = Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.BereketSigorta).FirstOrDefault().browser;
                    await BereketSigortaBrowser.LoadUrlAsync("https://nareks.bereket.com.tr/NonLife/Policy/SavePolicyCascoTraffic.aspx?APP_MP=T01");


                    var resp11 = await BereketSigortaBrowser.EvaluateScriptAsync("var createEvent = function(name) { var event = document.createEvent('Event'); event.initEvent(name, true, true); return event; }");


                    var resp = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('focus'));");
                    var resp4 = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').value=\"" + tcKimlikNo + "\";");

                    var resp13 = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('change'));");
                    var resp14 = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('input'));");
                    var resp15 = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('blur'));");
                    var resp16 = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('focus'));");

                    var resp7 = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').click();");
                    var resp5 = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTPlate').focus();");

                    var dogumTarihi = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                    var status = true;
                    while (status)
                    {
                        if (dogumTarihi.Result!=null)
                        {
                            if (dogumTarihi.Result.ToString() != "")
                            {

                                return dogumTarihi.Result.ToString();

                            }
                            else
                            {
                                dogumTarihi = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                            }
                        }
                        else
                        {
                            dogumTarihi = await BereketSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                        }
                    }
                    return "Hata Oluştu";
                    break;
                   case Constant.InsuranceConstants.CorpusSigorta:
                    ChromiumWebBrowser CorpusSigortaBrowser;
                    if (!Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.CorpusSigorta).Any())
                    {
                        return "Şirket Kapalı";
                    }
                    CorpusSigortaBrowser = Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.CorpusSigorta).FirstOrDefault().browser;
                    await CorpusSigortaBrowser.LoadUrlAsync("https://sigorta.corpussigorta.com.tr/NonLife/Policy/SavePolicy.aspx?APP_MP=TR1");

                    await CorpusSigortaBrowser.EvaluateScriptAsync("var createEvent = function(name) { var event = document.createEvent('Event'); event.initEvent(name, true, true); return event; }");
                 
                    await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('focus'));");
                    await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').value=\"" + tcKimlikNo + "\";");

                    await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('change'));");
                    await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('input'));");
                    await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('blur'));");
                    await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('focus'));");

                    await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').click();");

                    dogumTarihi = await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                     status = true;
                    while (status)
                    {
                        if (dogumTarihi.Result != null)
                        {
                            if (dogumTarihi.Result.ToString() != "")
                            {

                                return dogumTarihi.Result.ToString();

                            }
                            else
                            {
                                dogumTarihi = await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                            }
                        }
                        else
                        {
                            dogumTarihi = await CorpusSigortaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                        }
                    }
                    return "Hata Oluştu";
                    break;
                case Constant.InsuranceConstants.TurkiyeSigorta:
                    ChromiumWebBrowser TurkiyeSigortaBrowser;
                    if (!Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.TurkiyeSigorta).Any())
                    {
                        return "Şirket Kapalı";
                    }
                    TurkiyeSigortaBrowser = Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.TurkiyeSigorta).FirstOrDefault().browser;
                    await TurkiyeSigortaBrowser.LoadUrlAsync("https://pusula.turkiyesigorta.com.tr/modul/musteri/musteriBrowse.seam?menu_id=44");


                    await TurkiyeSigortaBrowser.EvaluateScriptAsync("document.getElementsByClassName('mukellefTuru')[0].children[0].children[0].children[0].children[0].click()");
                    Thread.Sleep(500);
                    await TurkiyeSigortaBrowser.EvaluateScriptAsync("document.getElementsByClassName('text tiny h_small girisAlani')[1].value=\"" + tcKimlikNo + "\";");
                    Thread.Sleep(500);

                    await TurkiyeSigortaBrowser.EvaluateScriptAsync("document.getElementsByClassName('button')[0].click()");
                    Thread.Sleep(500);

                    await TurkiyeSigortaBrowser.EvaluateScriptAsync("document.getElementsByClassName('rich-table-cell')[6].children[0].click()");


                    var status2 = true;
                    var checktarih = await TurkiyeSigortaBrowser.EvaluateScriptAsync("document.getElementsByClassName('rich-calendar-button')[0].innerText");

                    while (status2)
                    {
                        if (checktarih.Result != null)
                        {
                            var dtarih = await TurkiyeSigortaBrowser.EvaluateScriptAsync("document.getElementsByClassName('rich-calendar-input ')[0].value");
                            string tarih = dtarih.Result.ToString().Replace('/', '.');
                            return tarih;
                        }
                        else
                        {
                            checktarih = await TurkiyeSigortaBrowser.EvaluateScriptAsync("document.getElementsByClassName('rich-calendar-button')[0].innerText");

                        }
                    }
                    return "deneme";
                    break;
                case Constant.InsuranceConstants.NeovaSigorta:
                    ChromiumWebBrowser NeovaBrowser;
                    if (!Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.NeovaSigorta).Any())
                    {
                        return "Şirket Kapalı";
                    }
                    NeovaBrowser = Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.NeovaSigorta).FirstOrDefault().browser;
                    await NeovaBrowser.LoadUrlAsync("https://sigorta.neova.com.tr:5443/NonLife/Policy/SavePolicy.aspx?APP_MP=TR4");


                     resp11 = await NeovaBrowser.EvaluateScriptAsync("var createEvent = function(name) { var event = document.createEvent('Event'); event.initEvent(name, true, true); return event; }");


                    resp = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('focus'));");
                    resp4 = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').value=\"" + tcKimlikNo + "\";");

                    resp13 = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('change'));");
                    resp14 = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('input'));");
                    resp15 = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('blur'));");
                    resp16 = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('focus'));");

                    resp7 = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').click();");
                    resp5 = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTPlate').focus();");

                    dogumTarihi = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                    status = true;
                    while (status)
                    {
                        if (dogumTarihi.Result != null)
                        {
                            if (dogumTarihi.Result.ToString() != "")
                            {

                                return dogumTarihi.Result.ToString();

                            }
                            else
                            {
                                dogumTarihi = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                            }
                        }
                        else
                        {
                            dogumTarihi = await NeovaBrowser.EvaluateScriptAsync("document.getElementById('txtInsBirthdate').value");
                        }
                    }
                    return "Hata Oluştu";
                    break;
                case Constant.InsuranceConstants.TmtSigorta:
                    ChromiumWebBrowser TmtBrowser;
                    if (!Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.TmtSigorta).Any())
                    {
                        return "Şirket Kapalı";
                    }
                    TmtBrowser = Browser.webPageList.Where(x => x.insuranceCompany == InsuranceConstants.TmtSigorta).FirstOrDefault().browser;
                    await TmtBrowser.LoadUrlAsync("https://tmtsigorta.vizyoneks.com.tr/NonLife/Policy/SavePolicy.aspx?APP_MP=TUM");


                    resp11 = await TmtBrowser.EvaluateScriptAsync("var createEvent = function(name) { var event = document.createEvent('Event'); event.initEvent(name, true, true); return event; }");


                    resp = await TmtBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('focus'));");
                    resp4 = await TmtBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').value=\"" + tcKimlikNo + "\";");

                    resp13 = await TmtBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('change'));");
                    resp14 = await TmtBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('input'));");
                    resp15 = await TmtBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('blur'));");
                    resp16 = await TmtBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').dispatchEvent(createEvent('focus'));");

                    resp7 = await TmtBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTIdentityNo').click();");
                    resp5 = await TmtBrowser.EvaluateScriptAsync("document.getElementById('txtGIFTPlate').focus();");

                   dogumTarihi = await TmtBrowser.EvaluateScriptAsync("document.querySelector('#txtInsBirthDate').value");
                    status = true;
                    while (status)
                    {
                        if (dogumTarihi.Result != null)
                        {
                            if (dogumTarihi.Result.ToString() != "")
                            {

                                return dogumTarihi.Result.ToString();

                            }
                            else
                            {
                                dogumTarihi = await TmtBrowser.EvaluateScriptAsync("document.querySelector('#txtInsBirthDate').value");
                            }
                        }
                        else
                        {
                            dogumTarihi = await TmtBrowser.EvaluateScriptAsync("document.querySelector('#txtInsBirthDate').value");
                        }
                    }
                    return "Hata Oluştu";
                    break;
                default:
                    return "Çekilemedi";
                    break;
            }
        }
    }
}
