using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using SigortaVip.Models;
using System.Text.RegularExpressions;

namespace SigortaVip.Querry
{
    internal class DogaSigortaQuerry
    {

        public bool DogaSigorta(out HttpWebResponse response, QuerryItems querryItems)
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
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.sessionId[0] + "; _ga=GA1.2.1643197798.1653905449; _gid=GA1.2.1522335561.1653905449; _gat=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""acenteNo"":"""",""aKimlikNo"":""" + querryItems.tcKimlik + @""",""dogumTarihi"":""" + querryItems.dogumTarihi + @""",""ilIlceSorgula"":true,""urunKodu"":"""",""kisiTipi"":""""}";
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

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"EGMTramerSorgu");
                request.Referer = "https://portal.dogasigorta.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.sessionId[0] + "; _ga=GA1.2.767187755.1667631425; _gid=GA1.2.214012129.1667631425");

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
        public bool DogaSigortaVergiNoDetay(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.dogasigorta.com/ajaxpro/_default,ASWWebSite.ashx");

                request.ContentType = "text/plain; charset=utf-8";
                request.Accept = "*/*";
                request.Headers.Add("X-AjaxPro-Method", @"KisiIcinVergiNoBilgiAl");
                request.Referer = "https://portal.dogasigorta.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.sessionId[0] + "; _ga=GA1.2.1857204167.1667650923; _gid=GA1.2.1646312044.1667650923");

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
        public bool AracDogaSigortaVergiNo(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.dogasigorta.com/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.Accept = "*/*";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
                request.ContentType = "text/plain; charset=UTF-8";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.sessionId[0] + "; _ga=GA1.2.1857204167.1667650923; _gid=GA1.2.1646312044.1667650923; _gat=1");
                request.Headers.Add("DNT", @"1");
                request.Headers.Add("Origin", @"https://portal.dogasigorta.com");
                request.Referer = "https://portal.dogasigorta.com/default.aspx?p=urun~tumBranslar~police&policeIslemId=";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.182 Safari/537.36";
                request.Headers.Add("X-AjaxPro-Method", @"PlakadanHasarAlYeni");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":""" + querryItems.plakaKodu + @""",""plaka"":""" + querryItems.Plaka + @""",""sigortaliTcKimlikNo"":""" + querryItems.vergiNo + @""",""sigortaliVergiNo"":""" + querryItems.vergiNo + @""",""vadeBaslangic"":""" + DateTime.Now.ToString("dd.MM.yyyy") + @"""}";
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
        public bool AracDogaSigorta(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.dogasigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.KeepAlive = true;
                request.Headers.Add("sec-ch-ua", @""" Not A;Brand"";v=""99"", ""Chromium"";v=""101"", ""Google Chrome"";v=""101""");
                request.Headers.Add("X-AjaxPro-Method", @"TrafikPlakadanSorgulaV2");
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
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.sessionId[0] + "; _ga=GA1.2.1643197798.1653905449; _gid=GA1.2.1522335561.1653905449");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":""" + querryItems.plakaKodu + @""",""plaka"":""" + querryItems.Plaka + @""",""tcKimlikNo"":""" + querryItems.tcKimlik + @""",""vergiNo"":"""",""zeyilKesiliyor"":false,""aracTarz"":""""}";
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
        public bool DogaAracDetay(out HttpWebResponse response, QuerryItems querryItems)
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
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + querryItems.sessionId[0] + "; _ga=GA1.2.767187755.1667631425; _gid=GA1.2.214012129.1667631425; _gat=1");

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
        public bool DogaAracKalan(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                var sessions = querryItems.firmaSessionBilgileri;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.dogasigorta.com/ajaxpro/ASW_Is.HttpHandlers.PoliceHttpHandler,IsKatmani.ashx");

                request.Accept = "*/*";
                request.ContentType = "application/json";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"TrafikPlakadanSorgulaV3");
                request.Referer = "https://portal.dogasigorta.com/default.aspx?p=urun~tumBranslar~police&police&policeIslemId=15072023132433h5ihdnn5";
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + sessions.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First().ToString() + @"; NSC_wt_qpsubm.ephbtjhpsub.dpn=" + sessions.SessionList.Where(x => x.SessionName == "NSC_wt_qpsubm.ephbtjhpsub.dpn").Select(x => x.SessionValue).First().ToString() + @"; citrix_ns_id_.dogasigorta.com_%2F_wlf=" + sessions.SessionList.Where(x => x.SessionName == "citrix_ns_id_.dogasigorta.com_%2F_wlf").Select(x => x.SessionValue).First().ToString() + @"; citrix_ns_id_.dogasigorta.com_%2F_wat=" + sessions.SessionList.Where(x => x.SessionName == "citrix_ns_id_.dogasigorta.com_%2F_wat").Select(x => x.SessionValue).First().ToString() + @"; _ga=" + sessions.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First().ToString() + @"; _gid=" + sessions.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First().ToString() + @"; _gat=1");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"{""plakaIlKodu"":"""+querryItems.plakaKodu+@""",""plaka"":"""+querryItems.Plaka+@""",""tcKimlikNo"":"""+querryItems.tcKimlik+@""",""vergiNo"":"""+querryItems.tcKimlik+@""",""zeyilKesiliyor"":false,""aracTarz"":"""+querryItems.AracTarz+@""",""policeIslemId"":""""}";
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

        public bool DogaAracKod(out HttpWebResponse response, QuerryItems querryItems)
        {
            response = null;

            try
            {
                var sessions = querryItems.firmaSessionBilgileri;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://portal.dogasigorta.com/ajaxpro/UIModuller_urun_tumBranslar_kasko,ASWWebSite.ashx");

                request.Accept = "*/*";
                request.ContentType = "text/plain; charset=utf-8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr");
                request.Headers.Add("X-AjaxPro-Method", @"SasiSorgusundanAracKoduAl");
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + sessions.SessionList.Where(x => x.SessionName == "ASP.NET_SessionId").Select(x => x.SessionValue).First().ToString() + @"; NSC_wt_qpsubm.ephbtjhpsub.dpn=" + sessions.SessionList.Where(x => x.SessionName == "NSC_wt_qpsubm.ephbtjhpsub.dpn").Select(x => x.SessionValue).First().ToString() + @"; citrix_ns_id_.dogasigorta.com_%2F_wlf=" + sessions.SessionList.Where(x => x.SessionName == "citrix_ns_id_.dogasigorta.com_%2F_wlf").Select(x => x.SessionValue).First().ToString() + @"; citrix_ns_id_.dogasigorta.com_%2F_wat=" + sessions.SessionList.Where(x => x.SessionName == "citrix_ns_id_.dogasigorta.com_%2F_wat").Select(x => x.SessionValue).First().ToString() + @"; _ga=" + sessions.SessionList.Where(x => x.SessionName == "_ga").Select(x => x.SessionValue).First().ToString() + @"; _gid=" + sessions.SessionList.Where(x => x.SessionName == "_gid").Select(x => x.SessionValue).First().ToString() + @"; _gat=1");

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
    internal class DogaSigortaGetQuerry
    {
        public Models.Querry getDogaQuerry(QuerryItems querryItems)
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
            DogaSigortaQuerry dogaSigortaQuerry = new DogaSigortaQuerry();
            if (vergiVeTcKontrol != true)
            {
                if (dogaSigortaQuerry.DogaSigorta(out HttpWebResponse response, querryItems))
                {
                    try
                    {
                        string text = readResponse.getResponseText(response);
                        dynamic stuff = JsonConvert.DeserializeObject(text);

                        string adsoyad = stuff.value.AdUnvan + " " + stuff.value.Soyad;
                        string il = stuff.value.EvIlAdi;
                        string ilce = stuff.value.EvIlceAdi;
                        string adress = stuff.value.EvAdresi;
                        string CepTelefonu = stuff.value.CepTelefonu;
                        string EPosta = stuff.value.EPosta;

                        ReturnQuerry.adSoyad = adsoyad;
                        ReturnQuerry.Il = il;
                        ReturnQuerry.Ilce = ilce;
                        ReturnQuerry.Adress = adress;
                        ReturnQuerry.cepTelenu = CepTelefonu;
                        ReturnQuerry.Eposta = EPosta;
                    }
                    catch (Exception)
                    {

                        throw new Exception("Şirket Pencerenizi Kontrol Ediniz");
                    }

                    return ReturnQuerry;
                }
            }
            else
            {
                if (dogaSigortaQuerry.DogaSigortaVergiNo(out HttpWebResponse response, querryItems))
                {
                    try
                    {
                        string text = readResponse.getResponseText(response);

                        string x1 = text.Split(new string[] { "TrafikTescilTarihi\",\"Cevap\":\"" }, StringSplitOptions.None)[1];
                        string tescilTarihi = x1.Split('\"')[0];
                        ReturnQuerry.tescilTarihi = tescilTarihi;

                        x1 = text.Split(new string[] { "EGMAd\",\"Cevap\":\"" }, StringSplitOptions.None)[1];
                        string ad = x1.Split('\"')[0];

                        x1 = text.Split(new string[] { "YakitTipi\",\"Cevap\":\"" }, StringSplitOptions.None)[1];
                        string yakitTipi = x1.Split('\"')[0];

                        ReturnQuerry.adSoyad = ad;
                        ReturnQuerry.YakitTipi = yakitTipi;

                        var x3 = text.Split(new string[] { "EGMUstCins\",\"Cevap\":\"" }, StringSplitOptions.None)[1];

                        string kullanımsekli = x3.Split('\"')[0];
                        ReturnQuerry.kullanimSekli = kullanımsekli;
                        ReturnQuerry.kullanimTarzi = kullanımsekli;


                        if (dogaSigortaQuerry.DogaSigortaVergiNoDetay(out HttpWebResponse responses, querryItems))
                        {
                            try
                            {
                                text = readResponse.getResponseText(responses);
                                dynamic stuff = JsonConvert.DeserializeObject(text);

                                ReturnQuerry.Il = stuff.value.SbmIlAdi;
                                ReturnQuerry.Ilce = stuff.value.SbmIlceAdi;

                                return ReturnQuerry;
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("Şirket Pencerenizi Kontrol Ediniz");
                    }

                    return ReturnQuerry;
                }
            }
            throw new Exception("Bilgiler Çekilirken Bir Hata Oluştu");
        }
        public Models.Querry getDogaAracQuerry(QuerryItems querryItems)
        {
            Models.Querry ReturnQuerry = new Models.Querry();
            ReadResponse readResponse = new ReadResponse();

            DogaSigortaQuerry dogaSigortaQuerry = new DogaSigortaQuerry();
            if (querryItems.tcKimlik != "")
            {
                if (dogaSigortaQuerry.AracDogaSigorta(out HttpWebResponse response, querryItems))
                {
                    try
                    {
                        string text = readResponse.getResponseText(response);
                        string x1 = text.Split(new string[] { "\"marka\":{\"aciklama\":\"" }, StringSplitOptions.None)[1];
                        string marka = x1.Split('\"')[0];
                        ReturnQuerry.Marka = marka;
                        x1 = text.Split(new string[] { "\"kod\":\"" }, StringSplitOptions.None)[1];
                        string markakod = x1.Split('\"')[0];
                        x1 = text.Split(new string[] { "\"tip\":{\"aciklama\":\"" }, StringSplitOptions.None)[1];
                        string tip = x1.Split('\"')[0];
                        ReturnQuerry.Tip = tip;
                        x1 = text.Split(new string[] { "\"kod\":\"" }, StringSplitOptions.None)[2];
                        string tipkod = x1.Split('\"')[0];
                        ReturnQuerry.aracKod = markakod + tipkod;
                        x1 = text.Split(new string[] { "modelYili\":" }, StringSplitOptions.None)[1];
                        string model = x1.Split(',')[0];
                        ReturnQuerry.Model = model;
                        x1 = text.Split(new string[] { "motorNo\":" }, StringSplitOptions.None)[1];
                        string motorNo = x1.Split(',')[0];
                        motorNo = motorNo.Replace("\"", "");
                        ReturnQuerry.motorNo = motorNo;
                        x1 = text.Split(new string[] { "sasiNo\":" }, StringSplitOptions.None)[1];
                        string sasiNo = x1.Split(',')[0];
                        sasiNo = sasiNo.Replace("\"", "");
                        ReturnQuerry.sasiNo = sasiNo;

                        x1 = text.Split(new string[] { "uygulanmasiGerekenTarifeBasamakKodu\":" }, StringSplitOptions.None)[1];
                        string basamakKodu = x1.Split(',')[0];
                        ReturnQuerry.basamakKodu = basamakKodu;

                        x1 = text.Split(new string[] { ":\"GecmisPoliceVadeBaslangicTarihi" }, StringSplitOptions.None)[1];
                        string policebaslangicfirst = x1.Split('}')[0];
                        x1 = policebaslangicfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        string policebaslangic = x1.Split('\"')[0];
                        ReturnQuerry.policeBaslangis = policebaslangic;

                        x1 = text.Split(new string[] { ":\"GecmisPoliceVadeBitisTarihi" }, StringSplitOptions.None)[1];
                        string policebitisfirst = x1.Split('}')[0];
                        x1 = policebitisfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        string policebitis = x1.Split('\"')[0];

                        ReturnQuerry.policeBitis = policebitis;

                        x1 = text.Split(new string[] { ":\"AracTarz" }, StringSplitOptions.None)[1];
                        string AracTarzfirst = x1.Split('}')[0];
                        x1 = AracTarzfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        string AracTarz = x1.Split('\"')[0];

                        AracCinsi aracCinsi = new AracCinsi();
                        AracCinsi.AracCins arac = aracCinsi.AracCinsGetirSayiyaGore(AracTarz);
                        ReturnQuerry.kullanimTarzi = arac.Cins;

                        if (dogaSigortaQuerry.DogaAracDetay(out response, querryItems))
                        {
                            try
                            {
                                string AracDetayJson = readResponse.getResponseText(response);

                                x1 = AracDetayJson.Split(new string[] { ":\"EGMKullanimSekli" }, StringSplitOptions.None)[1];
                                string parsedFirst = x1.Split('}')[0];
                                x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string parsedtext = x1.Split('\"')[0];
                                if (parsedtext == "YOLCU NAKLI")
                                {
                                    parsedtext = "HUSUSI";
                                }
                                ReturnQuerry.kullanimSekli = parsedtext;

                                x1 = AracDetayJson.Split(new string[] { ":\"AracKisiSayisi" }, StringSplitOptions.None)[1];
                                string yolcuSayisifirst = x1.Split('}')[0];
                                x1 = yolcuSayisifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string yolcuSayisi = x1.Split('\"')[0];
                                ReturnQuerry.yolcuSayisi = yolcuSayisi;

                                x1 = AracDetayJson.Split(new string[] { ":\"YakitTipi" }, StringSplitOptions.None)[1];
                                string YakitTipifirst = x1.Split('}')[0];
                                x1 = YakitTipifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string YakitTipi = x1.Split('\"')[0];
                                ReturnQuerry.YakitTipi = YakitTipi;

                                x1 = AracDetayJson.Split(new string[] { ":\"TrafikTescilTarihi" }, StringSplitOptions.None)[1];
                                string TrafikTescilTarihifirst = x1.Split('}')[0];
                                x1 = TrafikTescilTarihifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string TrafikTescilTarihi = x1.Split('\"')[0];
                                ReturnQuerry.tescilTarihi = TrafikTescilTarihi;

                            }
                            catch (Exception)
                            {

                            }
                        }
                        response.Close();
                    }
                    catch (Exception)
                    {

                        throw new Exception("Şirket Pencerenizi Kontrol Ediniz");
                    }

                    return ReturnQuerry;
                }
            }
            else
            {


                aracsorgu aracsorgu = new aracsorgu();

                if (aracsorgu.DogaSigortaVergiNo(out HttpWebResponse response, querryItems))
                {
                    string text = readResponse.getResponseText(response);
                    string x1 = text.Split(new string[] { ":\"uygulanmasiGerekenTarifeBasamakKodu" }, StringSplitOptions.None)[1];
                    string parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                    string parsedtext = x1.Split('\"')[0];
                    ReturnQuerry.basamakKodu = parsedtext;

                    x1 = text.Split(new string[] { ":\"AracKodu" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                    parsedtext = x1.Split('\"')[0];
                    ReturnQuerry.aracKod = parsedtext;

                    x1 = text.Split(new string[] { ":\"GecmisPoliceVadeBaslangicTarihi" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                    parsedtext = x1.Split('\"')[0];
                    ReturnQuerry.policeBaslangis = parsedtext;

                    x1 = text.Split(new string[] { ":\"GecmisPoliceVadeBitisTarihi" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                    parsedtext = x1.Split('\"')[0];
                    ReturnQuerry.policeBitis = parsedtext;

                    x1 = text.Split(new string[] { ":\"MarkaAdi" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                    parsedtext = x1.Split('\"')[0];
                    ReturnQuerry.Marka = parsedtext;

                    x1 = text.Split(new string[] { ":\"TipAdi" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                    parsedtext = x1.Split('\"')[0];
                    ReturnQuerry.Tip = parsedtext;

                    x1 = text.Split(new string[] { "\"modelYili" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":" }, StringSplitOptions.None)[1];
                    parsedtext = x1.Replace(",","");
                    ReturnQuerry.Model = parsedtext;

                    x1 = text.Split(new string[] { ":\"AracTarz" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                    parsedtext = x1.Split('\"')[0];

                    AracCinsi aracCinsi = new AracCinsi();
                    AracCinsi.AracCins kullanimTarzi = aracCinsi.AracCinsGetirSayiyaGore(parsedtext);
                    ReturnQuerry.kullanimTarzi = kullanimTarzi.Cins;

                    x1 = text.Split(new string[] { ":\"AracKullanimSekli" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split('}')[0];
                    x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                    parsedtext = x1.Split('\"')[0];
                    ReturnQuerry.kullanimSekli = parsedtext;

                    x1 = text.Split(new string[] { "motorNo\":" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split(',')[0];
                    parsedtext = parsedFirst.Replace("\"", "");
                    ReturnQuerry.motorNo = parsedtext;

                    x1 = text.Split(new string[] { "sasiNo\":" }, StringSplitOptions.None)[1];
                    parsedFirst = x1.Split(',')[0];
                    parsedtext = parsedFirst.Replace("\"", "");
                    ReturnQuerry.sasiNo = parsedtext;

                    if (aracsorgu.DogaSigortaVergiNoDetay(out response,querryItems))
                    {
                        text = readResponse.getResponseText(response);

                        x1 = text.Split(new string[] { ":\"AracIstihapHaddiKisi" }, StringSplitOptions.None)[1];
                        parsedFirst = x1.Split('}')[0];
                        x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        parsedtext = x1.Split('\"')[0];
                        ReturnQuerry.yolcuSayisi = parsedtext;

                        x1 = text.Split(new string[] { ":\"YakitTipi" }, StringSplitOptions.None)[1];
                        parsedFirst = x1.Split('}')[0];
                        x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        parsedtext = x1.Split('\"')[0];
                        ReturnQuerry.YakitTipi = parsedtext;

                        x1 = text.Split(new string[] { ":\"TrafikTescilTarihi" }, StringSplitOptions.None)[1];
                        parsedFirst = x1.Split('}')[0];
                        x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        parsedtext = x1.Split('\"')[0];
                        ReturnQuerry.tescilTarihi = parsedtext;
                    }

                    return ReturnQuerry;
                }
            }
            throw new Exception("Bilgiler Çekilirken Bir Hata Oluştu");
        }

        public static string GetFieldValueFromJson(string json, string fieldName)
        {
            Match match = Regex.Match(json, $"\"{fieldName}\",\\{{\"SaltOkunur\":false,\"Soru\":\"{fieldName}\",\"Cevap\":\"([^\"]+)\"\\}}");
            return match.Success ? match.Groups[1].Value : "";
        }
    }
}
