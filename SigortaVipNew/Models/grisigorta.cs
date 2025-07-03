using System;
using System.IO;
using System.Net;

namespace SigortaVip.Models
{
    internal class grisigorta
	{
		public bool arackodu(out HttpWebResponse response, string sessionid2, string sessionid, string sasino)
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
				request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + sessionid + @"; _ga=GA1.3.363414647.1653943679; _gid=GA1.3.43823986.1653943679");

				request.Method = "POST";
				request.ServicePoint.Expect100Continue = false;

				string body = @"{""sasiNo"":""" + sasino + @"""}";
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
		public bool aracdetay(out HttpWebResponse response, string sessionid2, string sessionid, string arackodu)
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
				request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=" + sessionid + "; _ga=GA1.3.363414647.1653943679; _gid=GA1.3.43823986.1653943679");

				request.Method = "POST";
				request.ServicePoint.Expect100Continue = false;

				string body = @"{""aracKodu"":""" + arackodu + @"""}";
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
