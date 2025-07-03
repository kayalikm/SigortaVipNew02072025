using SigortaVip.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace SigortaVip.Querry
{
    internal class KoruSigortaQuerry
    {
        public bool KoruAracGetir(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                var sessions = querryItems.firmaSessionBilgileri;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.korusigortaportal.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://www.korusigortaportal.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + sessions.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First().ToString() + "; cookiesession1=" + sessions.SessionList.Where(x => x.SessionName == "cookiesession1").Select(x => x.SessionValue).First().ToString() + "; _gid=" + sessions.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First().ToString() + "; _ga_7Z4KQ98Y0H=" + sessions.SessionList.Where(x => x.SessionName == "_ga_7Z4KQ98Y0H").Select(x => x.SessionValue).First().ToString() + "; _ga=" + sessions.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First().ToString() + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":""" + querryItems.plakaKodu + @""",""plaka"":""" + querryItems.Plaka + @""",""tescilBelgeSeriKod"":""" + querryItems.belgeKodu + @""",""tescilBelgeSeriNo"":""" + querryItems.belgeNo + @""",""oncekiSirketKodu"":""000"",""oncekiAcenteKodu"":"""",""oncekiPoliceNo"":"""",""oncekiYenilemeNo"":""0"",""tcKimlikNo"":"""",""vergiNo"":"""",""pasaportNo"":"""",""zeyilKesiliyor"":false}";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
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

        public bool KoruAracKod(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                var sessions = querryItems.firmaSessionBilgileri;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.korusigortaportal.com//ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.Accept = "*/*";
                request.ContentType = "text/plain; charset=utf-8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"SasiSorgusundanAracKoduAl");
                request.Referer = "https://www.korusigortaportal.com//default.aspx?p=urun~tumBranslar~police&policeIslemId=31012021230758y8nzbe6p";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + sessions.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First().ToString() + "; cookiesession1=" + sessions.SessionList.Where(x => x.SessionName == "cookiesession1").Select(x => x.SessionValue).First().ToString() + "; _gid=" + sessions.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First().ToString() + "; _ga_7Z4KQ98Y0H=" + sessions.SessionList.Where(x => x.SessionName == "_ga_7Z4KQ98Y0H").Select(x => x.SessionValue).First().ToString() + "; _ga=" + sessions.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First().ToString() + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""sasiNo"":"""+querryItems.sasiNo+@"""}";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
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
    }
}
