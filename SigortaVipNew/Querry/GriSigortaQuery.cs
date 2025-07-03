using Newtonsoft.Json;
using SigortaVip.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
namespace SigortaVip.Querry
{
    internal class GriSigortaQuerry
    {

        public bool GriSigortaVergiNo(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.grisigorta.com.tr/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMSorgula");
                request.Referer = "https://portal.grisigorta.com.tr/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "; NSC_wt_qpsubm.hsjtjhpsub.dpn.us=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "NSC_wt_qpsubm.hsjtjhpsub.dpn.us").Select(x => x.SessionValue).First() + "; _gat=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":""" + querryItems.plakaKodu + @""",""plaka"":""" + querryItems.Plaka + @""",""tescilBelgeSeriKod"":""" + querryItems.belgeKodu + @""",""tescilBelgeSeriNo"":""" + querryItems.belgeNo + @"""}";
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
        public bool GriSigortaDetayVergiNo(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.grisigorta.com.tr/ajaxpro/_default,ASWWebSite.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Add("X-AjaxPro-Method", @"KisiIcinVergiNoBilgiAl");
                request.Referer = "https://portal.grisigorta.com.tr/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "; NSC_wt_qpsubm.hsjtjhpsub.dpn.us=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "NSC_wt_qpsubm.hsjtjhpsub.dpn.us").Select(x => x.SessionValue).First() + "; _gat=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""vergiNo"":""" + querryItems.vergiNo + @""",""kisiTipi"":""Sigortali"",""brans"":""340""}";
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
        public bool GriSigortaVergiNoAracKodu(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.grisigorta.com.tr/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.Accept = "*/*";
                request.ContentType = "text/plain; charset=utf-8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"SasiSorgusundanAracKoduAl");
                request.Referer = "https://portal.grisigorta.com.tr/default.aspx?p=urun~tumBranslar~police&policeIslemId=31012021230758y8nzbe6p";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "; NSC_wt_qpsubm.hsjtjhpsub.dpn.us=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "NSC_wt_qpsubm.hsjtjhpsub.dpn.us").Select(x => x.SessionValue).First() + "; _gat=1");

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
        public bool GriSigorta(out HttpWebResponse response, QuerryItems querryItems)
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
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "; NSC_wt_qpsubm.hsjtjhpsub.dpn.us=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "NSC_wt_qpsubm.hsjtjhpsub.dpn.us").Select(x => x.SessionValue).First() + "; _gat=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""tescilBelgeSeriKod"":""" + querryItems.belgeKodu + @""",""tescilBelgeSeriNo"":""" + querryItems.belgeNo + @""",""plakaIlKodu"":""" + querryItems.plakaKodu + @""",""plaka"":""" + querryItems.Plaka + @"""}";
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
        public bool GriSigortaAracKodu(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.grisigorta.com.tr/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.KeepAlive = true;
                request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
                request.Headers.Add("X-AjaxPro-Method", @"SasiSorgusundanAracKoduAl");
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
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "; NSC_wt_qpsubm.hsjtjhpsub.dpn.us=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "NSC_wt_qpsubm.hsjtjhpsub.dpn.us").Select(x => x.SessionValue).First() + "; _gat=1");

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
        public bool GriSigortaAracDetay(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.grisigorta.com.tr/ajaxpro/_default,ASWWebSite.ashx");

                request.KeepAlive = true;
                request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
                request.Headers.Add("X-AjaxPro-Method", @"KaskoIcinAlMarkaBilgisi");
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
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First() + "; _ga=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First() + "; _gid=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First() + "; NSC_wt_qpsubm.hsjtjhpsub.dpn.us=" + querryItems.firmaSessionBilgileri.SessionList.Where(x => x.SessionName == "NSC_wt_qpsubm.hsjtjhpsub.dpn.us").Select(x => x.SessionValue).First() + "; _gat=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""aracKodu"":""" + querryItems.aracKodu + @"""}";
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
    internal class GriSigortaGetQuerry
    {
        public Models.Querry getGriQuerry(QuerryItems querryItems)
        {
            Models.Querry ReturnQuerry = new Models.Querry();
            ReadResponse readResponse = new ReadResponse();
            string errorMessage = "";
            bool vergiVeTcKontrol = false;

            if (querryItems.tcKimlik == "" || querryItems.tcKimlik.Length != 11)
            {
                errorMessage += "TcKimlik boştur veya yanlış girilmiştir" + Environment.NewLine;
                vergiVeTcKontrol = true;
            }
            if (vergiVeTcKontrol == true)
            {
                if (querryItems.vergiNo == "" || querryItems.vergiNo.Length != 10)
                {
                    errorMessage += "Vergi No boştur veya yanlış girilmiştir" + Environment.NewLine;
                }
                errorMessage = "";
            }
            if (querryItems.sessionId == null)
            {
                errorMessage += "Ekran Kapalıdır";
            }
            if (querryItems.Plaka == "")
            {
                errorMessage += "Plaka boştur veya yanlış girilmiştir" + Environment.NewLine;
            }
            if (querryItems.plakaKodu == "")
            {
                errorMessage += "Plaka Kodu boştur veya yanlış girilmiştir" + Environment.NewLine;
            }
            if (querryItems.belgeNo == "")
            {
                errorMessage += "Belge No boştur veya yanlış girilmiştir" + Environment.NewLine;
            }
            if (querryItems.belgeKodu == "")
            {
                errorMessage += "Belge Kodu boştur veya yanlış girilmiştir" + Environment.NewLine;
            }
            if (querryItems.dogumTarihi == "")
            {
                if (vergiVeTcKontrol == false)
                {
                    errorMessage += "Dogum Tarihi boştur veya yanlış girilmiştir" + Environment.NewLine;
                }
            }
            if (errorMessage.Length > 0)
            {
                throw new Exception(errorMessage);
            }
            GriSigortaQuerry GriSigortaQuerry = new GriSigortaQuerry();

            if (GriSigortaQuerry.GriSigortaVergiNo(out HttpWebResponse response, querryItems))
            {
                try
                {
                    string text = readResponse.getResponseText(response);
                    dynamic stuff = JsonConvert.DeserializeObject(text);

                    string adsoyad = stuff.value.SonSahipAd;
                    string il = stuff.value.EvIlAdi;
                    string ilce = stuff.value.EvIlceAdi;
                    string adress = stuff.value.EvAdresi;
                    string CepTelefonu = stuff.value.CepTelefonu;
                    string EPosta = stuff.value.EPosta;

                    if (querryItems.tcKimlik != "" && querryItems.tcKimlik !=null)
                    {
                        adsoyad += " " + stuff.value.SonSahipSoyad;
                    }
                    ReturnQuerry.adSoyad = adsoyad;
                    ReturnQuerry.Adress = adress;
                    ReturnQuerry.cepTelenu = CepTelefonu;
                    ReturnQuerry.Eposta = EPosta;
                    ReturnQuerry.YakitTipi = stuff.value.YakitTipi;
                    ReturnQuerry.kullanimSekli = stuff.value.KullanimSekli;
                    ReturnQuerry.Model = stuff.value.ModelYili;
                    ReturnQuerry.kullanimTarzi = stuff.value.UstCins;
                    ReturnQuerry.sasiNo = stuff.value.SasiNo;
                    ReturnQuerry.motorNo = stuff.value.MotorNo;
                    ReturnQuerry.Marka = stuff.value.Marka;
                    ReturnQuerry.Tip = stuff.value.Tip;
                    ReturnQuerry.tescilTarihi = stuff.value.TescilTarihiStr;
                    if (GriSigortaQuerry.GriSigortaDetayVergiNo(out HttpWebResponse responses, querryItems))
                    {

                        text = readResponse.getResponseText(responses);
                        stuff = JsonConvert.DeserializeObject(text);
                        ReturnQuerry.Il = stuff.value.SbmIlAdi;
                        ReturnQuerry.Ilce = stuff.value.SbmIlceAdi;
                    }
                    return ReturnQuerry;
                }
                catch (Exception)
                {

                    throw new Exception("Şirket Pencerenizi Kontrol Ediniz");
                }

                return ReturnQuerry;
            }

            throw new Exception("Bilgiler Çekilirken Bir Hata Oluştu");
        }
        public Models.Querry getGriAracQuerry(QuerryItems querryItems)
        {
            Models.Querry ReturnQuerry = new Models.Querry();
            ReadResponse readResponse = new ReadResponse();

            GriSigortaQuerry griSigortaQuerry = new GriSigortaQuerry();
            if (querryItems.tcKimlik != "")
            {
                if (griSigortaQuerry.GriSigorta(out HttpWebResponse response, querryItems))
                {
                    grisigorta grisigorta = new grisigorta();
                    string text = readResponse.getResponseText(response);
                    dynamic stuff = JsonConvert.DeserializeObject(text);

                    ReturnQuerry.yolcuSayisi = stuff.value.KoltukSayisi;

                    ReturnQuerry.Marka = stuff.value.Marka;

                    ReturnQuerry.Model = stuff.value.ModelYili;

                    ReturnQuerry.kullanimSekli = stuff.value.KullanimSekli;

                    ReturnQuerry.motorNo = stuff.value.MotorNo;

                    ReturnQuerry.sasiNo = stuff.value.SasiNo;

                    ReturnQuerry.Tip = stuff.value.Cinsi;
                    if (griSigortaQuerry.GriSigortaAracKodu(out response, querryItems))
                    {
                        text = readResponse.getResponseText(response);
                        stuff = JsonConvert.DeserializeObject(text);
                        ReturnQuerry.aracKod = stuff.value.Mesaj;
                    }
                    querryItems.aracKodu = stuff.value.Mesaj;
                    if (griSigortaQuerry.GriSigortaAracDetay(out response,querryItems))
                    {
                        text = readResponse.getResponseText(response);
                        stuff = JsonConvert.DeserializeObject(text);
                        ReturnQuerry.Tip = stuff.value.TipAdi;
                        ReturnQuerry.Marka = stuff.value.MarkaAdi;
                    }

                    response.Close();
                    return ReturnQuerry;
                }

            }
            else
            {
                if (griSigortaQuerry.GriSigortaVergiNoAracKodu(out HttpWebResponse response, querryItems))
                {
                    string text = readResponse.getResponseText(response);
                    dynamic stuff = JsonConvert.DeserializeObject(text);

                    ReturnQuerry.aracKod = stuff.value.Mesaj;

                    querryItems.aracKodu = stuff.value.Mesaj;
                    if (griSigortaQuerry.GriSigortaAracDetay(out response, querryItems))
                    {
                        text = readResponse.getResponseText(response);
                        stuff = JsonConvert.DeserializeObject(text);
                        ReturnQuerry.Tip = stuff.value.TipAdi;
                        ReturnQuerry.Marka = stuff.value.MarkaAdi;
                    }
                    return ReturnQuerry;
                }
            }
            throw new Exception("Bilgiler Çekilirken Bir Hata Oluştu");
        }
    }
}
