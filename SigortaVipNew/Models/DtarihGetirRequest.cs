using Newtonsoft.Json;
using SigortaVip.Constant;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SigortaVip.Models
{
    internal class DtarihGetirRequest
    {
        private bool Request_cosmos_sompojapan_com_tr(out HttpWebResponse response, QuerryItems querryItems,FirmaSessionBilgileri firmaSessionBilgileri)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://cosmos.sompojapan.com.tr/Common.ashx?q=GetPersonalInfoByTckNo&identityOrTaxNo="+querryItems.tcKimlik+"");

                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.ContentType = "text/plain; charset=utf-8";
                //request.Headers.Set(HttpRequestHeader.Cookie, @".authz="+firmaSessionBilgileri.SessionList.Where(x=> x.SessionName == ".authz").Select(x=> x.SessionValue) +"; NSC_xxx-fkfoup-iuuqt="+firmaSessionBilgileri.SessionList.Where(x=> x.SessionName== "NSC_ESNS").Select(x=>x.SessionValue) +"");
                request.Headers.Add("DNT", @"1");
                request.Headers.Add("Sec-Fetch-Dest", @"empty");
                request.Headers.Add("Sec-Fetch-Mode", @"cors");
                request.Headers.Add("Sec-Fetch-Site", @"same-origin");
                request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.182 Safari/537.36";

                response = (HttpWebResponse)request.GetResponse();

                int x = 0;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                if (response != null) response.Close();
                return false;
            }

            return true;

        }

        public Task<string> D_tarihGetir(QuerryItems querryItems,FirmaSessionBilgileri firmaSessionBilgileri)
        {
            ReadResponse readResponse = new ReadResponse();
            DtarihGetir dtarihGetir = new DtarihGetir();


            switch (querryItems.FirmaAdi)
            {
                case Constant.InsuranceConstants.SompoJapanSigorta:
                    if (Request_cosmos_sompojapan_com_tr(out HttpWebResponse response, querryItems,firmaSessionBilgileri))
                    {
                        string text = readResponse.getResponseText(response);
                        dynamic stuff = JsonConvert.DeserializeObject(text);

                        return stuff.BirthDate;
                    }
                    break;
                case Constant.InsuranceConstants.BereketSigorta:
                    var dtarih = dtarihGetir.DtarihGetirAsync(InsuranceConstants.BereketSigorta, querryItems.tcKimlik);
                    if (dtarih.Result == "Şirket Kapalı" && dtarih.Result == "Hata Oluştu")
                    {
                    }
                    string[] tarih = dtarih.Result.ToString().Split('.');
                    if (Convert.ToInt16(tarih[0]) < 10)
                    {
                        return dtarih;
                    }
                    return dtarih;

                    break;
                case Constant.InsuranceConstants.CorpusSigorta:
                    dtarih = dtarihGetir.DtarihGetirAsync(InsuranceConstants.CorpusSigorta, querryItems.tcKimlik);
                    if (dtarih.Result == "Şirket Kapalı" && dtarih.Result == "Hata Oluştu")
                    {
                    }
                     tarih = dtarih.Result.ToString().Split('.');
                    if (Convert.ToInt16(tarih[0]) < 10)
                    {
                        return dtarih;
                    }
                    return dtarih;

                    break;
                case Constant.InsuranceConstants.TurkiyeSigorta:
                    dtarih = dtarihGetir.DtarihGetirAsync(InsuranceConstants.TurkiyeSigorta, querryItems.tcKimlik);
                    if (dtarih.Result == "Şirket Kapalı" && dtarih.Result == "Hata Oluştu")
                    {
                    }
                    tarih = dtarih.Result.ToString().Split('.');
                    if (Convert.ToInt16(tarih[0]) < 10)
                    {
                        return dtarih;
                    }
                    return dtarih;

                    break;

                default:
                    break;
            }
            return null;

        }
    }
}
