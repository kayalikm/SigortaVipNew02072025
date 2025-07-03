using System;
using System.IO;
using System.Net;

namespace SigortaVip.Models
{
    internal class sorgu
    {
        public bool KoruSigorta(out HttpWebResponse response, string sessionid, string sessionid2, string tckimlik, string dogumtarihi)
        {

            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.korusigortaportal.com/ajaxpro/_default,ASWWebSite.ashx");

                request.KeepAlive = true;
                request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
                request.Headers.Add("X-AjaxPro-Method", @"TcKimlikNoBilgisiAlTramerHasarSorgula");
                request.ContentType = "text/plain; charset=UTF-8";
                request.Headers.Add("sec-ch-ua-mobile", @"?0");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36";
                request.Headers.Add("sec-ch-ua-platform", @"""Windows""");
                request.Accept = "*/*";
                request.Headers.Add("Origin", @"https://www.korusigortaportal.com");
                request.Headers.Add("Sec-Fetch-Site", @"same-origin");
                request.Headers.Add("Sec-Fetch-Mode", @"cors");
                request.Headers.Add("Sec-Fetch-Dest", @"empty");
                request.Referer = "https://www.korusigortaportal.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=3005202211010799hieafv";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                request.Headers.Set(HttpRequestHeader.Cookie, @"cookiesession1=" + sessionid2 + "; _ga=GA1.2.1562726835.1653724503; _gid=GA1.2.1147973939.1653724503; ASP.NET_SessionId=" + sessionid + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""acenteNo"":"""",""aKimlikNo"":""" + tckimlik + @""",""dogumTarihi"":""" + dogumtarihi + @""",""ilIlceSorgula"":false,""urunKodu"":"""",""kisiTipi"":""""}";
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
        public bool DogaSigorta(out HttpWebResponse response, string sessionid, string sessionid2, string tckimlik, string dogumtarihi)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.dogasigorta.com/ajaxpro/_default,ASWWebSite.ashx");

                request.KeepAlive = true;
                request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
                request.Headers.Add("X-AjaxPro-Method", @"TcKimlikNoBilgisiAlTramerHasarSorgula");
                request.ContentType = "text/plain; charset=UTF-8";
                request.Headers.Add("sec-ch-ua-mobile", @"?0");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36";
                request.Headers.Add("sec-ch-ua-platform", @"""Windows""");
                request.Accept = "*/*";
                request.Headers.Add("Origin", @"https://portal.dogasigorta.com");
                request.Headers.Add("Sec-Fetch-Site", @"same-origin");
                request.Headers.Add("Sec-Fetch-Mode", @"cors");
                request.Headers.Add("Sec-Fetch-Dest", @"empty");
                request.Referer = "https://portal.dogasigorta.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=30052022131100k8rfzll8";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + sessionid + "; _ga=GA1.2.1643197798.1653905449; _gid=GA1.2.1522335561.1653905449; _gat=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""acenteNo"":"""",""aKimlikNo"":""" + tckimlik + @""",""dogumTarihi"":""" + dogumtarihi + @""",""ilIlceSorgula"":true,""urunKodu"":"""",""kisiTipi"":""""}";
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
        public bool GriSigorta(out HttpWebResponse response, string sessionid, string sessionid2, string tckimlik, string dogumtarihi)
        {

            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.grisigorta.com.tr/ajaxpro/_default,ASWWebSite.ashx");

                request.KeepAlive = true;
                request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
                request.Headers.Add("X-AjaxPro-Method", @"TcKimlikNoBilgisiAlTramerHasarSorgula");
                request.ContentType = "text/plain; charset=UTF-8";
                request.Headers.Add("sec-ch-ua-mobile", @"?0");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36";
                request.Headers.Add("sec-ch-ua-platform", @"""Windows""");
                request.Accept = "*/*";
                request.Headers.Add("Origin", @"https://portal.grisigorta.com.tr");
                request.Headers.Add("Sec-Fetch-Site", @"same-origin");
                request.Headers.Add("Sec-Fetch-Mode", @"cors");
                request.Headers.Add("Sec-Fetch-Dest", @"empty");
                request.Referer = "https://portal.grisigorta.com.tr/default.aspx?p=urun~tumBranslar~police&policeIslemId=30052022234812pbgomfg8";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + sessionid + "; _ga=GA1.3.363414647.1653943679; _gid=GA1.3.43823986.1653943679");


                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""acenteNo"":"""",""aKimlikNo"":""" + tckimlik + @""",""dogumTarihi"":""" + dogumtarihi + @""",""ilIlceSorgula"":false,""urunKodu"":"""",""kisiTipi"":""""}";
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
        public bool KoruSigortaVergiNo(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.korusigortaportal.com/ajaxpro/_default,ASWWebSite.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Add("X-AjaxPro-Method", @"KisiIcinVergiNoBilgiAl");
                request.Referer = "https://www.korusigortaportal.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList[0].SessionValue + "; cookiesession1=" + querryItems.firmaSessionBilgileri.SessionList[1].SessionValue + "; _ga=GA1.2.223935623.1676794491; _gid=GA1.2.1548409282.1676794491; _gat_gtag_UA_141923320_1=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""vergiNo"":""" + querryItems.vergiNo + @""",""kisiTipi"":""Sigortali"",""brans"":""310""}";
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
