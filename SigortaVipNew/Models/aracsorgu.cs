using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SigortaVip.Models
{
    internal class aracsorgu
    {
        public bool KoruSigorta(out HttpWebResponse response, string sessionid, string sessionid2, string tckimlik, string plaka, string plakakodu, string vergino)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.korusigortaportal.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.KeepAlive = true;
                request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
                request.Headers.Add("X-AjaxPro-Method", @"TrafikPlakadanSorgulaV2");
                request.ContentType = "text/plain; charset=UTF-8";
                request.Headers.Add("sec-ch-ua-mobile", @"?0");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36";
                request.Headers.Add("sec-ch-ua-platform", @"""Windows""");
                request.Accept = "*/*";
                request.Headers.Add("Origin", @"https://www.korusigortaportal.com");
                request.Headers.Add("Sec-Fetch-Site", @"same-origin");
                request.Headers.Add("Sec-Fetch-Mode", @"cors");
                request.Headers.Add("Sec-Fetch-Dest", @"empty");
                request.Referer = "https://www.korusigortaportal.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=30052022124145d3eeu589";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                request.Headers.Set(HttpRequestHeader.Cookie, @"cookiesession1=" + sessionid2 + "; _ga=GA1.2.1562726835.1653724503; _gid=GA1.2.1147973939.1653724503; ASP.NET_SessionId=" + sessionid + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":""" + plakakodu + @""",""plaka"":""" + plaka + @""",""tcKimlikNo"":""" + tckimlik + @""",""vergiNo"":""" + vergino + @""",""zeyilKesiliyor"":false,""aracTarz"":""00""}";
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
        public bool KoruSigortaDetay(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.korusigortaportal.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://www.korusigortaportal.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"cookiesession1=" + querryItems.firmaSessionBilgileri.SessionList[1].SessionValue + "; _ga=GA1.2.1562726835.1653724503; _gid=GA1.2.1147973939.1653724503; ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList[0].SessionValue + "");

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
        public bool GriSigorta(out HttpWebResponse response, string sessionid, string sessionid2, string belgekodu, string plaka, string plakakodu, string belgeno)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.grisigorta.com.tr/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.KeepAlive = true;
                request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
                request.Headers.Add("X-AjaxPro-Method", @"EGMSorgula");
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

                string body = @"{""tescilBelgeSeriKod"":""" + belgekodu + @""",""tescilBelgeSeriNo"":""" + belgeno + @""",""plakaIlKodu"":""" + plakakodu + @""",""plaka"":""" + plaka + @"""}";
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
        public bool AveonSigorta(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.aveonglobalsigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://portal.aveonglobalsigorta.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId="+querryItems.firmaSessionBilgileri.SessionList.Where(x=> x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() +"; _ga="+ querryItems.firmaSessionBilgileri.SessionList.Where(x=> x.SessionName == "_ga").Select(x=> x.SessionValue).First() + "; _gid="+ querryItems.firmaSessionBilgileri.SessionList.Where(x=> x.SessionName == "_gid").Select(x=>x.SessionValue).First() + "");

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
        public bool AveonSigortaDetay(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.aveonglobalsigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://portal.aveonglobalsigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

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
        public bool AveonSigortaVergiNo(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.aveonglobalsigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://portal.aveonglobalsigorta.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

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
        public bool AveonSigortaAracKod(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.aveonglobalsigorta.com/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.Accept = "*/*";
                request.ContentType = "text/plain; charset=utf-8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"SasiSorgusundanAracKoduAl");
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""sasiNo"":""" + querryItems.sasiNo + @"""}";
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
        public bool AveonSigortaVergiNoDetay(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.aveonglobalsigorta.com/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.Accept = "*/*";
                request.ContentType = "text/plain; charset=utf-8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"SasiSorgusundanAracKoduAl");
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""sasiNo"":""" + querryItems.sasiNo + @"""}";
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
        public bool AveonBilgi(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.aveonglobalsigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.Accept = "*/*";
                request.ContentType = "application/json";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"TrafikPlakadanSorgulaV3");
                request.Referer = "https://portal.aveonglobalsigorta.com/default.aspx?p=urun~tumBranslar~police&police&policeIslemId=24062023172531ars9kd68";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":"""+querryItems.plakaKodu+@""",""plaka"":"""+querryItems.Plaka+@""",""tcKimlikNo"":"""+querryItems.tcKimlik+@""",""vergiNo"":"""+querryItems.tcKimlik+@""",""zeyilKesiliyor"":false,""aracTarz"":"""",""policeIslemId"":""24062023172531ars9kd68""}";
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

        public bool AveonBilgiAdress(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.aveonglobalsigorta.com/ajaxpro/_default,ASWWebSite.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Add("X-AjaxPro-Method", @"TcKimlikNoBilgisiAlTramerHasarSorgula");
                request.Referer = "https://portal.aveonglobalsigorta.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""acenteNo"":"""",""aKimlikNo"":"""+querryItems.tcKimlik+@""",""dogumTarihi"":"""+querryItems.dogumTarihi+@""",""ilIlceSorgula"":true,""urunKodu"":""310"",""kisiTipi"":""Sigortali""}";
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.korusigortaportal.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://www.korusigortaportal.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList[0].SessionValue + "; cookiesession1=" + querryItems.firmaSessionBilgileri.SessionList[1].SessionValue + "; _ga=GA1.2.223935623.1676794491; _gid=GA1.2.1548409282.1676794491; _gat_gtag_UA_141923320_1=1");

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

        public bool KoruSigortaVergiNoDetay(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.korusigortaportal.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.Accept = "*/*";
                request.ContentType = "application/json";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"TrafikPlakadanSorgulaV3");
                request.Referer = "https://www.korusigortaportal.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=19022023111633zpdsng7i";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList[0].SessionValue + "; cookiesession1=" + querryItems.firmaSessionBilgileri.SessionList[1].SessionValue + "; _ga=GA1.2.223935623.1676794491; _gid=GA1.2.1548409282.1676794491; _gat_gtag_UA_141923320_1=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":""" + querryItems.plakaKodu + @""",""plaka"":""" + querryItems.Plaka + @""",""tcKimlikNo"":""" + querryItems.vergiNo + @""",""vergiNo"":""" + querryItems.vergiNo + @""",""zeyilKesiliyor"":false,""aracTarz"":""" + querryItems.AracTarz + @""",""policeIslemId"":""19022023111633zpdsng7i""}";
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

        public bool DogaSigortaVergiNo(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.dogasigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.Accept = "*/*";
                request.ContentType = "application/json";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"TrafikPlakadanSorgulaV3");
                request.Referer = "https://portal.dogasigorta.com/default.aspx?p=urun~tumBranslar~police&police&policeIslemId=19022023132346pptzkccp";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList[0].SessionValue + "; _ga=GA1.2.2061590660.1676792337; _gid=GA1.2.1068181787.1676792337; NSC_wt_qpsubm.ephbtjhpsub.dpn=" + querryItems.firmaSessionBilgileri.SessionList[1].SessionValue + "; _gat=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":""" + querryItems.plakaKodu + @""",""plaka"":""" + querryItems.Plaka + @""",""tcKimlikNo"":""" + querryItems.vergiNo + @""",""vergiNo"":""" + querryItems.vergiNo + @""",""zeyilKesiliyor"":false,""aracTarz"":"""",""policeIslemId"":""19022023132346pptzkccp""}";
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

        public bool DogaSigortaVergiNoDetay(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.dogasigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://portal.dogasigorta.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.First().SessionValue + @"; _ga=GA1.2.2061590660.1676792337; _gid=GA1.2.1068181787.1676792337; NSC_wt_qpsubm.ephbtjhpsub.dpn=" + querryItems.firmaSessionBilgileri.SessionList[1].SessionValue + "; _gat=1");

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

        public bool AnaSigorta(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                
                List<FirmaSessionBilgileri.Session> sessions = querryItems.firmaSessionBilgileri.SessionList;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.anasigorta.com.tr/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://portal.anasigorta.com.tr/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":"""+querryItems.plakaKodu+@""",""plaka"":"""+querryItems.Plaka+@""",""tescilBelgeSeriKod"":"""+querryItems.belgeKodu+@""",""tescilBelgeSeriNo"":"""+querryItems.belgeNo+@""",""oncekiSirketKodu"":""000"",""oncekiAcenteKodu"":"""",""oncekiPoliceNo"":"""",""oncekiYenilemeNo"":""0"",""tcKimlikNo"":"""",""vergiNo"":"""",""pasaportNo"":"""",""zeyilKesiliyor"":false}";
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

        public bool AnaSigortaDetay(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.anasigorta.com.tr/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.Accept = "*/*";
                request.ContentType = "text/plain; charset=utf-8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"SasiSorgusundanAracKoduAl");
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

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

        public bool AnaSigortaLast(out HttpWebResponse response, QuerryItems querryItems, AracCinsi.AracCins arac)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.anasigorta.com.tr/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.Accept = "*/*";
                request.ContentType = "application/json";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"TrafikPlakadanSorgulaV3");
                request.Referer = "https://portal.anasigorta.com.tr/default.aspx?p=urun~tumBranslar~police&police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":"""+querryItems.plakaKodu+@""",""plaka"":"""+querryItems.Plaka+@""",""tcKimlikNo"":"""+querryItems.tcKimlik+@""",""vergiNo"":"""+querryItems.tcKimlik+@""",""zeyilKesiliyor"":false,""aracTarz"":"""+arac.Number+@""",""policeIslemId"":""""}";
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

        public bool AnaSigortaBilgi(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.anasigorta.com.tr/ajaxpro/_default,ASWWebSite.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Add("X-AjaxPro-Method", @"TcKimlikNoBilgisiAlTramerHasarSorgula");
                request.Referer = "https://portal.anasigorta.com.tr/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""acenteNo"":"""",""aKimlikNo"":"""+querryItems.tcKimlik+@""",""dogumTarihi"":"""+querryItems.dogumTarihi+@""",""ilIlceSorgula"":true,""urunKodu"":""310"",""kisiTipi"":""Sigortali""}";
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
